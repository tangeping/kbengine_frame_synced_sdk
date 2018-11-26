# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class FS_FrameData(list):
    """
    """
    def __init__(self):
        """
        """
        list.__init__(self)

    def asDict(self):
        data = {
            "frameid"   : self[0],
            "operation" : self[1],
        }
        return data

    def createFromDict(self, dictData):
        self.extend([dictData["frameid"],dictData["operation"]])
        return self


class FS_FRAME_DATA_PICKLER:
    def __init__(self):
        pass

    def createObjFromDict(self, dict):
        return FS_FrameData().createFromDict(dict)

    def getDictFromObj(self, obj):
        return obj.asDict()

    def isSameType(self, obj):
        return isinstance(obj, FS_FrameData)


inst = FS_FRAME_DATA_PICKLER()



