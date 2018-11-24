# -*- coding: utf-8 -*-
import KBEngine
import GlobalDefine
from KBEDebug import *

import math
import time
import copy
import SCDefine

from ENTITY_DATA import TEntityFrame
from FRAME_DATA import TFrameData
from FRAME_LIST import TFrameList


class Frame(KBEngine.EntityComponent):
	def __init__(self):
		KBEngine.EntityComponent.__init__(self)
		self.avatars = {}


	def onAttached(self, owner):
		"""
		"""
		self.initFrameData()
		INFO_MSG("Operation::onAttached(): owner=%i" % (owner.id))

	def onDetached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onDetached(): owner=%i" % (owner.id))


	def initFrameData(self):
		'''
		初始化帧的数据
		'''
		self.farmeId = 1
		operation = TEntityFrame().createFromDict({"entityid":0,"cmd_type":0,"datas":b''})
		self.emptyFrame = TFrameData().createFromDict({"frameid":0,"operation":[operation]})
		self.currFrame = copy.deepcopy(self.emptyFrame)
		
	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.getScriptName(), self.id, tid, userArg))
		if userArg == SCDefine.TIMER_TYPE_SPACE_TICK:
			self.broadFrame()


	def addPlayer(self, entityCall):
		"""
		defined method.
		添加玩家
		"""

		DEBUG_MSG('Space::onEnter space[%d] entityID = %i.' % (self.spaceID, entityCall.id))

		
		entity = KBEngine.entities.get(entityCall.id,None)
		if entity:
			self.avatars[entityCall.id] = entity
			entity.component1.seatNo = len(self.avatars)


	def removePlayer(self, entityID):
		"""
		defined method.
		移除玩家
		"""

		DEBUG_MSG('Space::onLeave space[%d] entityID = %i.' % (self.spaceID, entityID))
		
		if entityID in self.avatars:
			del self.avatars[entityID]

	def start(self):
		"""
		开始帧同步
		"""
		self.addTimer(1,0.00001,SCDefine.TIMER_TYPE_SPACE_TICK)
		
		self.state = GlobalDefine.FRAME_STATE_RUNNING

	def stop(self):
		"""
		停止帧同步
		"""
		if self.state == GlobalDefine.FRAME_STATE_RUNNING:
			self.state = GlobalDefine.FRAME_STATE_STOP

	def reportFrame(self,entityCall, framedata):
		"""
		添加数据帧
		"""		
		if entityCall is None or self.state != GlobalDefine.FRAME_STATE_RUNNING:
			return

		operation = TEntityFrame().createFromDict({"entityid":framedata[0],"cmd_type":framedata[1],"datas":framedata[2]})

		if self.currFrame[0] <= 0:			
			self.currFrame[1] = [operation]
		else:
			self.currFrame[1].append(operation)	

	def broadFrame(self):
		"""
		广播逻辑帧
		"""
		if self.state != GlobalDefine.FRAME_STATE_RUNNING:
			return

		self.currFrame[0] = self.farmeId
		self.framePool[self.farmeId] = self.currFrame

		for e in self.avatars.values():
			if e is None or e.client is None:
				continue
			for frameid in range(e.frameId,self.spaceFarmeId):
				e.component1.client.onFrameMessage(self.framePool[frameid+1])
				
			e.component1.frameId = self.farmeId

		
		self.currFrame = copy.deepcopy(self.emptyFrame)
		self.farmeId += 1

