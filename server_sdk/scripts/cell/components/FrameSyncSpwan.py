# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class FrameSyncSpwan(KBEngine.EntityComponent):
	def __init__(self):
		KBEngine.EntityComponent.__init__(self)
		self.activeEntity = None

	def onAttached(self, owner):
		"""
		"""			
		INFO_MSG("Operation::onAttached(): owner=%i" % (owner.id))

		self.activeEntity = KBEngine.createEntity("FrameSyncMgr", self.spaceID, tuple(self.position), tuple(self.direction), {})

	def onDetached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onDetached(): owner=%i" % (owner.id))






