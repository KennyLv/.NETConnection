package com.webfin.websocket.server;

import java.io.IOException;
import java.nio.CharBuffer;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONObject;
import com.webfin.websocket.EventConst;
import com.webfin.websocket.entity.DeviceAccount;
import com.webfin.websocket.event.BaseEvent;
import com.webfin.websocket.handle.DBEventHandleImpl;

public class WebSocketMessageInboundPool {

	//保存A类设备和B类设备客户端http长连接的MAP容器
	private static final ArrayList<WebSocketMessageInbound> connections = new ArrayList<WebSocketMessageInbound>();

	//向连接池中添加连接
	public static void addMessageInbound(WebSocketMessageInbound inbound){
		//添加连接
		DeviceAccount da = inbound.getConnectedDevice();
		System.out.println("新上线设备 : " + da.getDeviceNm());
		connections.add(inbound);
	}

	//获取所有的连线的设备
	public static ArrayList<DeviceAccount> getOnlineDevices(){
		ArrayList<DeviceAccount> onlineDevices = new ArrayList<DeviceAccount>();
		for(WebSocketMessageInbound webClient:connections)
		{
			onlineDevices.add(webClient.getConnectedDevice());
		}
		return onlineDevices;
	}

	//获取在线的某一group的B设备名称
	public static WebSocketMessageInbound getGroupBDevices(String group){
		WebSocketMessageInbound retWebClient =null;
		for(WebSocketMessageInbound webClient:connections)
		{
			if(webClient.getConnectedDevice().getDeviceGroup().equals(group)&&webClient.getConnectedDevice().getType().equals("B")){
				retWebClient =  webClient;
			}
		}
		return retWebClient;
	}

	//fastJson转换有bug，只能转换为JSONObject的Array，需要重新处理为DeviceAccount对象Array
//	private static ArrayList<DeviceAccount> parseJsonArray(ArrayList jsonArray){
//		ArrayList<DeviceAccount> lsEventObj = new ArrayList<DeviceAccount>();
//		for(int i=0,len=jsonArray.size();i<len;i++){
//			JSONObject jsonObj = (JSONObject)jsonArray.get(i);
//			DeviceAccount item = (DeviceAccount)JSONObject.toJavaObject(jsonObj,DeviceAccount.class);
//			lsEventObj.add(item);
//		}
//		return lsEventObj;		
//	}

	//A类设备或者B类设备离线退出（服务端收到客户端关闭websocket连接事件，触发WebSocketInbound中的onClose方法），从连接池中删除连接设备客户端的连接实例
	public static void removeMessageInbound(WebSocketMessageInbound inbound){
		//移除连接
		System.out.println("设备离线 : " + inbound.getConnectedDevice());
		connections.remove(inbound);
	}

	//处理客户端消息
	public static void processTextMessage(WebSocketMessageInbound inbound,String message){
		//1:解码客户端msg		
		//2:根据msg构造Event事件
		//3:调用EventHandle处理(开线程）
		//4:处理完后向客户端返回
		//5:如果是服务端返回计算事件，向该组B设备推送消息，否则向发送消息的客户端退总消息
		BaseEvent receiveEvent = (BaseEvent)JSON.parseObject(message.toString(),BaseEvent.class);
		//(需要处理fastJson框架JSONObject转换的bug)
//		ArrayList<DeviceAccount> lsEventObj = parseJsonArray(receiveEvent.getEventObjs());
//		receiveEvent.setEventObjs(lsEventObj);
		DBEventHandleImpl dbEventHandle = new DBEventHandleImpl();
		dbEventHandle.setReceiveEvent(receiveEvent);
		dbEventHandle.HandleEvent();
		if(receiveEvent.getEventType()==EventConst.EVENT_MATCHMATIC_RESULT||
				receiveEvent.getEventType()==EventConst.EVENT_GROUP_DEVICES_RESULT||
				receiveEvent.getEventType()==EventConst.EVENT_A_REPAIRE){
			String clientDeviceGroup = ((ArrayList<DeviceAccount>)receiveEvent.getEventObjs()).get(0).getDeviceGroup();
			WebSocketMessageInbound bClient = getGroupBDevices(clientDeviceGroup);
			if(bClient!=null){
				sendMessageToSingleClient(bClient,dbEventHandle.getReceiveEvent());
			}		

		}
		else{
//			if(inbound!=null&&receiveEvent.getEventType()!=EventConst.EVENT_A_LOGIN&&
//					receiveEvent.getEventType()!=EventConst.EVENT_B_LOGIN&&
//					receiveEvent.getEventType()!=EventConst.EVENT_B_LOGOFF&&
//					receiveEvent.getEventType()!=EventConst.EVENT_A_LOGOFF)
//			sendMessageToSingleClient(inbound,dbEventHandle.getReceiveEvent());

		}

	}


	//发送数据给所有在线设备
	public static void sendMessageToAllDevices(BaseEvent event){
		try {
			for (WebSocketMessageInbound webClient : connections) {
				webClient.sendMessage(event);

			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	//向某一客户端发送数据
	public static void sendMessageToSingleClient(WebSocketMessageInbound webClient,BaseEvent event){

		try {
			webClient.sendMessage(event);

		}
		catch (Exception e) {
			e.printStackTrace();
		}

	}

}
