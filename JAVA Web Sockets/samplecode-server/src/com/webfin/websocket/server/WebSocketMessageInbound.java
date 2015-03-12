package com.webfin.websocket.server;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.CharBuffer;
import javax.servlet.http.HttpServletRequest;
import org.apache.catalina.websocket.MessageInbound;
import org.apache.catalina.websocket.WsOutbound;

import com.alibaba.fastjson.JSON;
import com.webfin.websocket.EventConst;
import com.webfin.websocket.dao.DeviceAccountDao;
import com.webfin.websocket.entity.DeviceAccount;
import com.webfin.websocket.event.BaseEvent;
import com.webfin.websocket.handle.DBEventHandleImpl;

public class WebSocketMessageInbound extends MessageInbound {

	//当前连接的用户名称
	private final HttpServletRequest request;
	private DeviceAccount connectedDevice;

	//get/set method
	public DeviceAccount getConnectedDevice() {
		return connectedDevice;
	}


	public void setConnectedDevice(DeviceAccount connectedDevice) {
		this.connectedDevice = connectedDevice;
	}


	public HttpServletRequest getRequest() {
		return request;
	}


	public WebSocketMessageInbound(HttpServletRequest request) {
		this.request = request;
		DeviceAccount connectedDa = (DeviceAccount)request.getSession(true).getAttribute("connectedDevice");
		if(connectedDa==null)
		{
			String deviceId = request.getParameter("id");
			DeviceAccountDao deviceDao = new DeviceAccountDao();
			connectedDa = deviceDao.getDaById(Integer.parseInt(deviceId));
		}
		this.setConnectedDevice(connectedDa);
	}

	
	//建立连接的触发的事件
	@Override
	protected void onOpen(WsOutbound outbound) {
    	//向客户端发送回调以建立握手
//		WebSocketMessageInboundPool.sendMessageToSingleClient(this,new BaseEvent(EventConst.EVENT_A_LOGIN));

	}

	@Override
	protected void onClose(int status) {
		// 触发关闭事件，在连接池中移除连接
		WebSocketMessageInboundPool.removeMessageInbound(this);
		
	}

	@Override
	protected void onBinaryMessage(ByteBuffer message) throws IOException {
		throw new UnsupportedOperationException("Binary message not supported.");
	}

	//客户端发送消息到服务器时触发事件，调用WebSocketMessageInboundPool的processTextMessage统一处理A类设备的登入，更新位置，离线等消息
	@Override
	protected void onTextMessage(CharBuffer message) throws IOException {
		WebSocketMessageInboundPool.processTextMessage(this, message.toString());
		
	}
	
	//向客户端发送数据，使用Websocket WsOutbound输出流向客户端退送数据，数据格式统一为Json
	public void sendMessage(BaseEvent event)
	{
		String eventStr = JSON.toJSONString(event);
		try {
			//向连接设备发送数据
			System.out.println("send message to device : " + this.getConnectedDevice().getDeviceNm() + " ,event content(json format) : " + eventStr);
			this.getWsOutbound().writeTextMessage(CharBuffer.wrap(eventStr));			
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
}
