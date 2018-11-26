# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class FrameSyncReport(KBEngine.EntityComponent):
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

		frameSyncMgr = getFrameSyncMgr()
		if frameSyncMgr is not None:
			frameSyncMgr.removePlayer(owner.id)

	def getFrameSyncMgr(self):
		"""
		获得当前space的entity baseEntityCall
		"""
		return KBEngine.cellAppData.get("FrameSync_%i" % self.spaceID,None)

	def reportFrame(self,exposed,framedata):
		"""
		上传操作帧
		"""
		if exposed != self.id:
			return

		self.getFrameSyncMgr().reportFrame(self.owner,framedata)







