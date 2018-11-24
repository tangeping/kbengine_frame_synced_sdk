# -*- coding: utf-8 -*-
import KBEngine
import GlobalDefine
from KBEDebug import *

class Operation(KBEngine.EntityComponent):
	def __init__(self):
		KBEngine.EntityComponent.__init__(self)

	def onAttached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onAttached(): owner=%i" % (owner.id))

	def onDetached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onDetached(): owner=%i" % (owner.id))


	def reportFrame(self,exposed,framedata):
		"""
		上传操作帧
		"""
		if exposed != self.id:
			return

		self.getCurrSpace().component1.reportFrame(self.owner,framedata)


