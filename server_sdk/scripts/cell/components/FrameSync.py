# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class OperationSync(KBEngine.EntityComponent):
	def __init__(self):
		KBEngine.EntityComponent.__init__(self)

	def onAttached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onAttached(): owner=%i" % (owner.id))

		getFrameSyncMgr().addPlayer(self)

	def onDetached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onDetached(): owner=%i" % (owner.id))

		getFrameSyncMgr().removePlayer(owner.id)

	def getFrameSyncMgr(self):
		"""
		获得当前space的entity baseEntityCall
		"""
		return KBEngine.globalData["FrameSync_%i" % self.spaceID]

	def reportFrame(self,exposed,framedata):
		"""
		上传操作帧
		"""
		if exposed != self.id:
			return

		self.getFrameSyncMgr().reportFrame(self.owner,framedata)


	def  start(self,exposed):

			if exposed != self.id:
			return

		self.getFrameSyncMgr().start()	

	def  stop(self,exposed):

			if exposed != self.id:
			return

		self.getFrameSyncMgr().stop()	




