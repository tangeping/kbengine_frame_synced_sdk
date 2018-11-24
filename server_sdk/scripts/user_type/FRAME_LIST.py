# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *


class TFrameList(dict):
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


class FRAME_LIST_PICKLER:
    def __init__(self):
        pass

    def createObjFromDict(self, dict):
        return TFrameList().createFromDict(dict)

    def getDictFromObj(self, obj):
        return obj.asDict()

    def isSameType(self, obj):
        return isinstance(obj, TFrameList)


inst = FRAME_LIST_PICKLER()