# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class FrameSync(KBEngine.EntityComponent):
	def __init__(self):
		KBEngine.EntityComponent.__init__(self)
		self.frameMgr = None

	def onAttached(self, owner):
		"""
		"""			
		INFO_MSG("Operation::onAttached(): owner=%i" % (owner.id))

		self.frameMgr = KBEngine.createEntity("FrameSyncMgr", self.owner.spaceID, tuple( self.owner.position), tuple( self.owner.direction), {})

	def onDetached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onDetached(): owner=%i" % (owner.id))


	def start(self):
		'''
		帧同步开始
		'''
		if self.frameMgr:
			self.frameMgr.start()


	def stop(self):
		'''
		帧同步结束
		'''
		if self.frameMgr:
			self.frameMgr.stop()



