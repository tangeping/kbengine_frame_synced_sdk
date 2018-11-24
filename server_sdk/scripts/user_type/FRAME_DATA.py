# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class TFrameData(list):
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


class FRAME_DATA_PICKLER:
    def __init__(self):
        pass

    def createObjFromDict(self, dict):
        return TFrameData().createFromDict(dict)

    def getDictFromObj(self, obj):
        return obj.asDict()

    def isSameType(self, obj):
        return isinstance(obj, TFrameData)


inst = FRAME_DATA_PICKLER()


'''class TFrameData(dict):
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


class FRAME_DATA_PICKLER:
    def __init__(self):
        pass

    def createObjFromDict(self, dict):
        return TFrameData().createFromDict(dict)

    def getDictFromObj(self, obj):
        return obj.asDict()

    def isSameType(self, obj):
        return isinstance(obj, TFrameData)
inst = FRAME_DATA_PICKLER()
'''
