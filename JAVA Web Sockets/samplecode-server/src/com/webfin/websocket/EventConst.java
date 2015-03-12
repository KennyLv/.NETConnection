package com.webfin.websocket;

public  class EventConst {

	//客户端发送的消息类型
	public static final int EVENT_A_LOGIN = 1;
	public static final int EVENT_B_LOGIN = 2;
	public static final int EVENT_MATHMATIC = 3;
	public static final int EVENT_A_UPDATING = 4;
	public static final int EVENT_B_UPDATING = 5;
	public static final int EVENT_A_LOGOFF = 6;
	public static final int EVENT_B_LOGOFF = 7;
	public static final int EVENT_A_REPAIRE =8;
	//服务端返回的消息类型
	
	public static final int EVENT_DEVICE_NOT_EXSITE=101;
	public static final int EVENT_DEVICE_EXSITE=102;
	public static final int EVENT_MATCHMATIC_RESULT=103;
	public static final int EVENT_GROUP_DEVICES_RESULT=104;
	
	
}
