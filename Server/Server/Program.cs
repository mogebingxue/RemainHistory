﻿using System;

namespace Game
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//连接数据库，如果数据库连接失败则直接结束程序
 			if(!DbManager.Connect("game", "127.0.0.1", 3306, "root", "@Yao05090610")){
 				return;
 			}
			//连接数据库成功则进入服务器主循环
			NetManager.StartLoop(8888);
		}
	}
}
