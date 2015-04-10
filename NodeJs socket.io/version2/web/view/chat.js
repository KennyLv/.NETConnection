


function appendMessage(msg) {
	$("#talkFrame").append("<div class='appenedMessage'>" + msg + "</div>");
	/*
	JS控制滚动条的位置：
	window.scrollTo(x,y);
	竖向滚动条置顶(window.scrollTo(0,0);
	竖向滚动条置底 window.scrollTo(0,document.body.scrollHeight)

	JS控制TextArea滚动条自动滚动到最下部
	document.getElementById('textarea').scrollTop = document.getElementById('textarea').scrollHeight
	*/
	$("#talkFrame").scrollTop = $("#talkFrame").scrollHeight;
}


$(document).ready(function() {
	
	if (typeof WebSocket === 'undefined') {
		$("#loginPage").hide();
		$("#errorPage").show();
	}
	
	$("#open").click(function(event) {
		currentUserNick = $.trim($("#nickInput").val());
		if ('' == currentUserNick) {
			alert('请先输入昵称');
			return;
		}
		
		$("#onlineUsers").html("");
		$("#talkFrame").html("");
		$("#nickInput").val("");
		
		onlineUserMap = null;
		currentUser = null;
		
		resetSocketConnection();
		
		clientSocket = new clientSocketInstance();
		clientSocket.startSession(socketAddress,socketEventListener);
	});

	$("#message").keyup(function(event) {
		if (13 == event.keyCode) {
			var value = $.trim($("#message").val());
			if (value) {
				$("#message").val('');
				clientSocket.sendMsg( EVENT_TYPE.COMMON_EVENT.PUBLIC_SPEAK , value);
			}
		}
	});
	
	$("#send").click(function(event) {
		var value = $.trim($("#message").val());
		if (value) {
			$("#message").val('');
			clientSocket.sendMsg( EVENT_TYPE.COMMON_EVENT.PUBLIC_SPEAK , value);
		}
	});
	
	$("#logout").click(function(event){
		clientSocket.sendMsg(EVENT_TYPE.COMMON_EVENT.LOGOUT, currentUser);
	});
	
	$(".userName").click(function(event){
		console.log("userName on click");
		console.log( event );
	});
	
	
});
