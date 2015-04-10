var zTool = require("./zTool");
var onlineUserMap = new zTool.SimpleMap();
var historyContent = new zTool.CircleList(100);

var chatLib = require("./chatLib");
var EVENT_TYPE = chatLib.USER_EVENT;
var PORT = chatLib.PORT;

//TODO : 分端口，游戏跟私聊使用不同的端口
//使用socket.io直接启动http服务
var io = require("socket.io").listen(PORT);

io.sockets.on("connection",function(socket){
		socket.on("message",function(message){
				var mData = chatLib.analyzeMessageData(message);
				if (mData && mData.EVENT) {
						switch (mData.EVENT) {
							case EVENT_TYPE.COMMON_EVENT.LOGIN: // 新用户连接
								var newUser = {'uid':socket.id, 'nick':chatLib.getMsgFirstDataValue(mData)};
								// 把新连接的用户增加到在线用户列表
								onlineUserMap.put(socket.id, newUser);
								// 把新用户的信息广播给在线用户
								var data = JSON.stringify({
										'EVENT' : EVENT_TYPE.COMMON_EVENT.LOGIN,
										'user':onlineUserMap.get(socket.id),
										'users':onlineUserMap.values(),
										'historyContent':historyContent.values()
								});
								io.sockets.emit('message',data);//广播
								//socket.emit('message',data);// socket.broadcast.emit('message', data);//无效
							break;
							
							case EVENT_TYPE.COMMON_EVENT.CREATE_ROOM: //用户创建房间
								
							break;
							
							case EVENT_TYPE.COMMON_EVENT.ENTER_ROOM: //用户进入房间
								
							break;
							
							case EVENT_TYPE.COMMON_EVENT.PUBLIC_SPEAK: //用户发言
								var content = chatLib.getMsgFirstDataValue(mData);
								var data = JSON.stringify({
										'EVENT' : EVENT_TYPE.COMMON_EVENT.PUBLIC_SPEAK,
										'user':onlineUserMap.get(socket.id),
										'values' : [content]
								});
								//socket.emit('message',data);
								io.sockets.emit('message',data);
								historyContent.add({'user':onlineUserMap.get(socket.id),'content':content,'time':new Date().getTime()});
							break;

							case EVENT_TYPE.COMMON_EVENT.VIEW_HISTORY://用户主动获取历史记录
								//TODO : 返回一定时间段内的历史记录
							break;
							
							case EVENT_TYPE.COMMON_EVENT.LEAVE_ROOM: //用户离开房间/群组
								//TODO : 如果所有人都退出，则删除房间
							break;
							
							case EVENT_TYPE.COMMON_EVENT.LOGOUT: // 用户退出
								var user = mData.values[0];
								onlineUserMap.remove(user.uid);
								var data = JSON.stringify({
										'EVENT' : EVENT_TYPE.COMMON_EVENT.LOGOUT,
										'livingUser' : user
								});
								io.sockets.emit('message',data);
								//TODO : 通过socket.id断开连接？
							break;

							default:
							break;
						}

				} else {
					// 事件类型出错，记录日志，向当前用户发送错误信息
					console.log('desc:message,userId:' + socket.id + ',message:' + message);
					var data = JSON.stringify({
						'EVENT' : EVENT_TYPE.COMMON_EVENT.UN_PARSEABLE_MESSAGE,
						'uid':socket.id
					});
					socket.emit('message',data);
				}
		});

});

console.log('Start listening on port ' + PORT);