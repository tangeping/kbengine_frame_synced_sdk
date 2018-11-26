# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

import math
import time
import copy

from FS_ENTITY_DATA import FS_EntityFrame
from FS_FRAME_DATA import FS_FrameData
from FS_FRAME_LIST import FS_FrameList


# ------------------------------------------------------------------------------
# frame state
# ------------------------------------------------------------------------------

FS_STATE_FREE  		= 0
FS_STATE_RUNNING 	= 1
FS_STATE_STOP 		= 2

# ------------------------------------------------------------------------------
# frame state
# ------------------------------------------------------------------------------
FS_TIMER_TYPE_DESTROY									= 99999999 # 延时销毁entity


class FrameSyncMgr(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)

		self.players = {}

		KBEngine.globalData["FrameSyncMgr_%i" % self.spaceID] = self


	def initFrameData(self):
		'''
		初始化帧的数据
		'''
		self.farmeID = 1
		operation = FS_EntityFrame().createFromDict({"entityid":0,"cmd_type":0,"datas":b''})
		self.emptyFrame = FS_FrameData().createFromDict({"frameid":0,"operation":[operation]})
		self.currFrame = copy.deepcopy(self.emptyFrame)
		
	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.getScriptName(), self.id, tid, userArg))
		if userArg == FS_TIMER_TYPE_DESTROY:
			self.broadFrame()


	def addPlayer(self, frameSyncComponent):
		"""
		defined method.
		添加玩家
		"""

		DEBUG_MSG('Space::onEnter space[%d] entityID = %i.' % (self.spaceID, frameSyncComponent.ownerID))

		

		self.players[frameSyncComponent.ownerID] = frameSyncComponent

		frameSyncComponent.seatNo = len(self.players)


	def removePlayer(self, entityID):
		"""
		defined method.
		移除玩家
		"""

		DEBUG_MSG('Space::onLeave space[%d] entityID = %i.' % (self.spaceID, entityID))
		
		if entityID in self.players:
			del self.players[entityID]

	def start(self):
		"""
		开始帧同步
		"""
		self.addTimer(1,0.00001,FS_TIMER_TYPE_DESTROY)
		
		self.state = FS_STATE_RUNNING

	def stop(self):
		"""
		停止帧同步
		"""
		if self.state == FS_STATE_RUNNING:
			self.state = FS_STATE_STOP

	def reportFrame(self,entityCall, framedata):
		"""
		添加数据帧
		"""		
		if entityCall is None or self.state != FS_STATE_RUNNING:
			return

		operation = FS_EntityFrame().createFromDict({"entityid":framedata[0],"cmd_type":framedata[1],"datas":framedata[2]})

		if self.currFrame[0] <= 0:			
			self.currFrame[1] = [operation]
		else:
			self.currFrame[1].append(operation)	

	def broadFrame(self):
		"""
		广播逻辑帧
		"""
		if self.state != FS_STATE_RUNNING:
			return

		self.currFrame[0] = self.farmeID
		self.framePool[self.farmeID] = self.currFrame

		for p in self.players.values():
			for frameid in range(p.farmeID,self.farmeID):
				p.client.onFrameMessage(self.framePool[frameid+1])
				
			p.farmeID = self.farmeID

		
		self.currFrame = copy.deepcopy(self.emptyFrame)
		self.farmeID += 1

