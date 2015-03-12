package com.webfin.websocket.event;

import java.io.Serializable;
import java.util.ArrayList;

import com.alibaba.fastjson.JSONObject;
import com.webfin.websocket.EventConst;
import com.webfin.websocket.entity.DeviceAccount;

public class BaseEvent implements Serializable{
	
	private int eventType=EventConst.EVENT_DEVICE_NOT_EXSITE ;
	private ArrayList<DeviceAccount> eventObjs;
	
	
	public ArrayList<DeviceAccount> getEventObjs() {
		return eventObjs;
	}
	public void setEventObjs(ArrayList<DeviceAccount> eventObjs) {
		this.eventObjs = eventObjs;
	}
	public int getEventType() {
		return eventType;
	}
	public void setEventType(int eventType) {
		this.eventType = eventType;
	}
	
	public BaseEvent(){
	
	}
	
	public BaseEvent(int eventType){
		this.setEventType(eventType);
//		ArrayList<Integer> test = new ArrayList<Integer>();
//		this.setEventObjs(test);
		
	}
	
	

}
