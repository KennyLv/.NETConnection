package com.webfin.websocket.handle;

import com.webfin.websocket.event.*;

public interface  EventHandle {
		
	//处理消息,所有子类必须overide
	public void HandleEvent( );
		

}
