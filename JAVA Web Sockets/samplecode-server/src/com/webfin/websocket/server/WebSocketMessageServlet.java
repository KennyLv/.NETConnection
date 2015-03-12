package com.webfin.websocket.server;

import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServletRequest;

import org.apache.catalina.websocket.StreamInbound;

import com.webfin.websocket.entity.DeviceAccount;

//@WebServlet(urlPatterns = { "/ly/"})
//如果要接收浏览器的ws://协议的请求就必须实现WebSocketServlet这个类
public class WebSocketMessageServlet extends org.apache.catalina.websocket.WebSocketServlet {

	private static final long serialVersionUID = 1L;
	
	
	//A类设备或者B类设备发起连接后，服务端的接口函数，跟平常Servlet不同的是，在该函数中需要实现createWebSocketInbound，类似SocketServer的accept方法，新生产的WebSocketInbound实例对应客户端http长连接，处理与客户端交互功能，
	//在这里初始化自定义的WebSocket连接对象，该对象会将查询A类设备在后台数据库的详细数据并做为WebSocketMessageInbound的成员变量
	//将新的WebSocketInbound实例加入自定义的WebSocketMessageInboundPool连接池中，以便统一处理A/B设备组员关系及位置分布信息计算等业务逻辑
    @Override
    protected StreamInbound createWebSocketInbound(String subProtocol,HttpServletRequest request) {
    	
    	WebSocketMessageInbound newClientConn =  new WebSocketMessageInbound(request);
        WebSocketMessageInboundPool.addMessageInbound(newClientConn);
        return newClientConn;
        
    }


	
}
