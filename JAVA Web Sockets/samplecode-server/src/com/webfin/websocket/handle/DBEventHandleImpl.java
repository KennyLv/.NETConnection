package com.webfin.websocket.handle;

import java.util.ArrayList;

import com.webfin.websocket.EventConst;
import com.webfin.websocket.dao.DeviceAccountDao;
import com.webfin.websocket.entity.DeviceAccount;
import com.webfin.websocket.event.BaseEvent;

public class DBEventHandleImpl implements EventHandle,Runnable {

	private DeviceAccountDao daDao = new DeviceAccountDao();
	private BaseEvent receiveEvent = null;
	private BaseEvent responseEvent = null;
	private boolean isFinish = false;
	
	//get/set
	public boolean isFinish() {
		return isFinish;
	}

	public void setFinish(boolean isFinish) {
		this.isFinish = isFinish;
	}

	public BaseEvent getReceiveEvent() {
		return receiveEvent;
	}

	public void setReceiveEvent(BaseEvent receiveEvent) {
		this.receiveEvent = receiveEvent;
	}
	

	public BaseEvent getResponseEvent() {
		return responseEvent;
	}

	public void setResponseEvent(BaseEvent responseEvent) {
		this.responseEvent = responseEvent;
	}
	
	
		

	@Override
	public void HandleEvent() {
		int eventType = this.receiveEvent.getEventType();
		ArrayList<DeviceAccount> lsDevices = (ArrayList<DeviceAccount>)this.receiveEvent.getEventObjs();

		//A/B类设备登录事件
		//1：更新其status为在线，并更新地理位置信息，返回该设备数据
		if(eventType==EventConst.EVENT_A_LOGIN||eventType==EventConst.EVENT_B_LOGIN){
			DeviceAccount da = (DeviceAccount)lsDevices.get(0);
			da.setStatus("online");
			daDao.update(da);
		}

		//A设备保修事件
		//1：查询该设备信息
		//2：更新该设备地理位置，并重新计算该设备组目标位置
		if(eventType==EventConst.EVENT_A_REPAIRE){
			DeviceAccount da = (DeviceAccount)lsDevices.get(0);			
			daDao.update(da);
			DeviceAccount targetDa = daDao.mathTargetDevice(da);
			ArrayList<DeviceAccount> groupDevices = daDao.getGroupDevices(da);
			lsDevices.clear();
			lsDevices.addAll(groupDevices);
			lsDevices.add(targetDa);
			this.receiveEvent.setEventType(EventConst.EVENT_MATCHMATIC_RESULT);
			this.receiveEvent.setEventObjs(lsDevices);

		}
		//B设备移动事件
		//1：查询该设备信息
		//2：重新计算该设备组的目标位置
		if(eventType==EventConst.EVENT_B_UPDATING){
			DeviceAccount da = (DeviceAccount)lsDevices.get(0);			
			da.setStatus("online");
			daDao.update(da);
			DeviceAccount targetDa = daDao.mathTargetDevice(da);
			ArrayList<DeviceAccount> groupDevices = daDao.getGroupDevices(da);
			lsDevices.clear();
			lsDevices.addAll(groupDevices);
			if(targetDa.getLat()!=0&&targetDa.getLat()!=0){
				lsDevices.add(targetDa);
			}			
			this.receiveEvent.setEventType(EventConst.EVENT_MATCHMATIC_RESULT);
			this.receiveEvent.setEventObjs(lsDevices);

		}
		//计算事件
		if(eventType==EventConst.EVENT_MATHMATIC){
			DeviceAccount da = (DeviceAccount)lsDevices.get(0);
			DeviceAccount targetDa = daDao.mathTargetDevice(da);
			ArrayList<DeviceAccount> groupDevices = daDao.getGroupDevices(da);
			lsDevices.clear();
			lsDevices.addAll(groupDevices);
			lsDevices.add(targetDa);
			this.receiveEvent.setEventType(EventConst.EVENT_MATCHMATIC_RESULT);
			this.receiveEvent.setEventObjs(lsDevices);


		}
		//A或者B类设备掉线
		//1:更新状态
		//2:重新计算同组目标位置
		if(eventType==EventConst.EVENT_A_LOGOFF)
		{		
			DeviceAccount da = (DeviceAccount)lsDevices.get(0);
			da.setStatus("offline");
			daDao.update(da);
			DeviceAccount targetDa = daDao.mathTargetDevice(da);
			ArrayList<DeviceAccount> groupDevices = daDao.getGroupDevices(da);
			lsDevices.clear();
			lsDevices.addAll(groupDevices);
			lsDevices.add(targetDa);
			this.receiveEvent.setEventObjs(lsDevices);
			this.receiveEvent.setEventType(EventConst.EVENT_MATCHMATIC_RESULT);
			
		}
		//B设备掉线
		if(eventType==EventConst.EVENT_B_LOGOFF){
			DeviceAccount da = (DeviceAccount)lsDevices.get(0);
			da.setStatus("offline");
			daDao.update(da);
		}
		this.isFinish=true;


	}

	@Override
	public void run() {
		// TODO Auto-generated method stub
		this.HandleEvent();
	}

}
