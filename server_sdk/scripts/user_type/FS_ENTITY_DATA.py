# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *


class FS_EntityData(list):
    """
    """
    def __init__(self):
        """
        """
        list.__init__(self)

    def asDict(self):
        data = {
            "entityid"  : self[0],
            "cmd_type"   : self[1],
            "datas"     : self[2],
        }
        return data

    def createFromDict(self, dictData):
        self.extend([dictData["entityid"], dictData["cmd_type"], dictData["datas"]])
        return self


class FS_ENTITY_DATA_PICKLER:
    def __init__(self):
        pass

    def createObjFromDict(self, dict):
        return FS_EntityData().createFromDict(dict)

    def getDictFromObj(self, obj):
        return obj.asDict()

    def isSameType(self, obj):
        return isinstance(obj, FS_EntityData)


inst = FS_ENTITY_DATA_PICKLER()


