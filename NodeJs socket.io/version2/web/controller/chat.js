
var currentUser = null;
var currentUserNick = null;
var onlineUsers = null;

var EVENT_TYPE = chatLib.USER_EVENT;

var clientSocket = null;
var socketAddress = chatLib.HOST + ":" + chatLib.PORT;

var socketEventListener = {
	"onSocketConnected" : this.onNotifySocketConnected,
	"onSocketError" : this.onNotifySocketError,
	"onSocketClosed" : this.onNotifySocketClosed,
	"onSocketMessageError" : this.onNotifyServerMessageError,
	"onSocketMessageReceived" : this.onNotifyServerMessageReceived
};

function onNotifySocketConnected(){
	appendMessage("<span class='notification'>[网络已连接，登录中...] </span>");
	clientSocket.sendMsg( EVENT_TYPE.COMMON_EVENT.LOGIN , currentUserNick);
}
function onNotifySocketError(){
	appendMessage("<span class='notification'>[网络出错啦，请稍后重试...] </span>");
}
function onNotifySocketClosed(){
	appendMessage("<span class='notification'>[网络连接已被关闭...] </span>");
}
function onNotifyMessageError(){
	appendMessage("<span class='notification'>[收到无法解析的数据...] </span>");
}

function onNotifyServerMessageReceived(mData){

	switch (mData.EVENT) {
		case EVENT_TYPE.COMMON_EVENT.LOGIN: // 新用户连接
			onNotifyUserLogin(mData);
		break;
		case EVENT_TYPE.COMMON_EVENT.LOGOUT: // 用户退出
			onNotifyUserLogout(mData);
		break;
		case EVENT_TYPE.COMMON_EVENT.PUBLIC_SPEAK: // 用户发言
			onNotifyNewMessage(mData);
		break;
		default:
		break;
	}
	
	/*
	var callBacks = {
		EVENT_TYPE.COMMON_EVENT.LOGIN  : this.onNotifyUserLogin,
		EVENT_TYPE.COMMON_EVENT.LOGOUT : this.onNotifyUserLogout,
		EVENT_TYPE.COMMON_EVENT.PUBLIC_SPEAK  : this.onNotifyNewMessage,
		EVENT_TYPE.COMMON_EVENT.UN_PARSEABLE_MESSAGE  : this.onNotifyMessageError
	};
	this.callBacks[mData.EVENT](mData);
	*/
}


function onNotifyUserLogin(mData){
	if(currentUser == null){
		$("#loginPage").hide();
		$("#chatPage").show();
		//如果是当前用户，获取最近的历史消息
		currentUser = mData.user;
		updateHistoryMessage(mData.historyContent);
	}
	appendMessage("<span class='notification'>" + formatUserTalkString(mData.user) + "[进入房间] </span>");
	//显示当前所有用户
	onlineUsers = mData.users;
	updateOnlineUser();
}

function onNotifyNewMessage(mData){
	var content = mData.values[0];
	appendMessage(formatUserTalkString(mData.user));
	appendMessage("<span>&nbsp;&nbsp;"+ content +"</span>");
}

function onNotifyUserLogout(mData){
	var livingUser = mData.userInfo;
	if(livingUser.uid == currentUser.uid){
		resetSocketConnection();
		$("#loginPage").show();
		$("#chatPage").hide();
	}else{
		//TODO : 删除该用户信息
		//onlineUsers.remove(livingUser.uid);
		updateOnlineUser();
		appendMessage("<span class='notification'>" + formatUserTalkString(user) + "[离开房间] </span>");
	}
}

function updateOnlineUser() {
	var html = ["<div>在线用户(" + onlineUsers.length + ")</div>"];
	if (onlineUsers.length > 0) {
		var number = onlineUsers.length;
		for ( var i=0;i<number;i++) {
			html.push("<div class='userName'>");
			if (onlineUsers[i].uid == currentUser.uid) {
				html.push("<b>" + formatUserString(onlineUsers[i]) + "(我)</b>");
			} else {
				html.push(formatUserString(onlineUsers[i]));
			}
			html.push("</div>");
		}
	}
	$("#onlineUsers").html(html.join(''));
}


function updateHistoryMessage(data) {
	if (data && data.length) {
	    var number = data.length;
	    for ( var i=0;i<number;i++) {
			appendMessage(formatUserTalkHisString(data[i].user, data[i].time));
			appendMessage("<span>&nbsp;&nbsp;" + data[i].content + "</span>");
	    }
	    appendMessage("<span class='notification'>==================以上为最近的历史消息==================</span>");
	}
}

function resetSocketConnection() {
	if (clientSocket) {
		clientSocket.terminateSession();
	}
	clientSocket = null;
}

function formatUserString(user) {
	if (!user) {
		return '';
	}
	return user.nick; //+ "<span class='gray'>(" + user.uid + ")</span> ";
}

function formatUserTalkString(user) {
	return formatUserString(user) + new Date().format("hh:mm:ss") + " ";
}

function formatUserTalkHisString(user, time) {
	return formatUserString(user) + new Date(time).format("yyyy-MM-dd hh:mm:ss") + " ";
}

