# 一、概述
### 1.背景：你点开了这个游戏，忽然白光一闪（播到这儿，此处应闪白光），你死了，哦！不，你昏了过去，你醒来发现五千年的历史不见了，没有历史，也就没了你。作为伟大的穿越者，你要找回丢失的历史，拼凑成回到21世纪的路。
# 二、游戏特色
### 1.玩家主人公可以选择种族，可以在战斗中选择或更换职业，主人公会积攒天赋，对队伍产生加成
### 2.羁绊：阵营和思想，以春秋战国诸国和诸子百家思想为羁绊。（寻古-先秦篇）
### 3.棋子：历史人物的化身。每个单位都拥有独特的技能，每种羁绊都需要足够多的棋子，升星机制，效仿自走棋，等级机制，因为无限的关卡模式，需要设置等级以适应关卡强度，同时等级所需经验应非线性增长（指数），或者说战力非线性增长（对数），保持别真的无限玩下去，越想关卡多肯定越难。
### 4.主角：主角有四个卡槽位，分别继承插入卡的阵营、思想、属性（一定算法）、技能。主角可以在开始一局游戏时通过拜祭设置阵营卡和思想卡，属性卡和技能卡在进入游戏后获得。
### 5.灵器：一些传说中的或者记载着中华文化的东西，负载了历史人物或者历史事件的精神，化为灵器。他们可以提供强大的属性或者特别的作用。灵器可以装备在棋子单位身上。每种灵器在一局游戏中只会出现一次，以保证玩家有足够多的决策。（比如越王勾践剑，比如兰亭集序（时间不对），比如苏武牧羊的节旄，蔺相如使秦王为赵王敲缶的瓦缶，介子推足下的木屐等，通过这些以科普，玩一些高雅的梗。）
### 6.背包（暂定名）：背包中可以存储一定数量的灵器和棋子。
### 7.等级：以队伍等级为基准，通过基础属性和成长属性来区分前期和后期角色，另一方面减少换阵容的代价。
# 三、游戏流程
### 1.闪屏，最好手写一个shader（放弃）
### 2.加载界面，tips可以写一些知识，例如：《尔雅》是辞书之祖，也是典籍——经，《十三经》的一种，是汉族传统文化的核心组成部分。
### 3.登录注册
### 4.主界面：一局游戏的入口，凌烟阁（请神归来的先人），知音（好友），世界聊天，尔雅（图鉴）
### 5.开始游戏，一些基础选择
### 6.游戏
### 7.结束游戏，结算
# 四、札记
### 1.显示\隐藏pawn装备（云顶的战斗和平时两种模式的显示）
### 2.操作模式：RTS操作，框选式选择，辅以快捷键，鼠标移动到边界镜头移动，战争迷雾，探索式显现小地图，点击敌人自动战斗
# 五、技术
### 1.数据库使用MongoDB
### 2.网络同步协议使用kcp
### 3.编码使用protobuf
### 4.代码热更新使用csharp.lua
### 5.文件校验使用MD5
### 改：变为2D，主要精力应放在服务端上，2D的移动战斗还有战争迷雾都简单许多
# 六、UI
## （一）主界面
### 1.凌烟阁：记载了你所达成的人物，出战时可以选择跟随你和附身你的人物。同时你可以在凌烟阁中拜祭先贤，获得他们的赐福，百分比的提升你队伍的能力。
### 2.尔雅：记载了你所收集过的灵器的图鉴。
### 3.知音：与你同样在寻找历史的同道中人，即好友。
## （二）游戏界面
左侧是羁绊，左上角角色头像、等级、经验条，头像下面显示宝物，头像内角落显示自己现在的羁绊。点击头像角色加背包界面。右上角设置，包含音乐、离开本次游戏。
