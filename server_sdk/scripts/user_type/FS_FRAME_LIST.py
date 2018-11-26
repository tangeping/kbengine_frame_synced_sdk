# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *


class FS_FrameList(dict):
    """
    """
    def __init__(self):
        """
        """
        dict.__init__(self)

    def asDict(self):

        datas = []
        dic = {
            "values": datas,
        }

        for key,val in self.items():
            datas.append(val)

        return dic

    def createFromDict(self, dictData):
        for data in dictData["values"]:
            self[data[0]] = data
        return self


class FS_FRAME_LIST_PICKLER:
    def __init__(self):
        pass

    def createObjFromDict(self, dict):
        return FS_FrameList().createFromDict(dict)

    def getDictFromObj(self, obj):
        return obj.asDict()

    def isSameType(self, obj):
        return isinstance(obj, FS_FrameList)


inst = FS_FRAME_LIST_PICKLER()