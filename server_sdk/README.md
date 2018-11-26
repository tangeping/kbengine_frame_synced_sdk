这是一个KBEngine 服务器帧同步组件开发包
========

##配置文件

使用 <gameUpdateHertz>30</gameUpdateHertz>  固定频率参数
	
<baseapp>  <externalUdpPorts_min> 和 <externalUdpPorts_max> 配置UDP端口，使用UDP通信


##包目录

cell: cell 部分下的组件目录 

entity_defs: 定义的组件声明目录

user_type: 定义的组件数据类型目录


如果需要用到帧同步,请将相对应的目录添加到您的对应的资产文件夹下面。

使用时只需要将改组件挂载到对应的 entity 即可使用
