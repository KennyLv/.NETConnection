package com.webfin.websocket.entity;

public class DeviceAccount {
	
	private int    id = 0;
	private String type = "A";
	private String deviceGroup = "group1";
	private String deviceNm = "deviceA_1";
	private String devicePw = "welcome1";
	private double lng = 0.0;
	private double lat = 0.0;
	private String status ="notExsit";
	
	
	public DeviceAccount()
	{
		
	}

	
	public DeviceAccount(String type,String group,String accountNm,String accountPw,double lng,double lat,String status)
	{
		this.setType(type);
		this.setDeviceGroup(group);
		this.setDeviceNm(accountNm);
		this.setDevicePw(accountPw);
		this.setLng(lng);
		this.setLat(lat);
		this.setStatus(status);
		
	}
	
	//get/set method
	public String getDeviceNm() {
		return deviceNm;
	}


	public void setDeviceNm(String deviceNm) {
		this.deviceNm = deviceNm;
	}


	public String getDevicePw() {
		return devicePw;
	}


	public void setDevicePw(String devicePw) {
		this.devicePw = devicePw;
	}
	
	public String getDeviceGroup() {
		return deviceGroup;
	}


	public void setDeviceGroup(String deviceGroup) {
		this.deviceGroup = deviceGroup;
	}
	
	public int getId() {
		return id;
	}


	public void setId(int id) {
		this.id = id;
	}
	
	public String getStatus() {
		return status;
	}

	public void setStatus(String status) {
		this.status = status;
	}
	
	public String getType() {
		return type;
	}
	public void setType(String type) {
		this.type = type;
	}
	
	
	public double getLng() {
		return lng;
	}
	public void setLng(double lng) {
		this.lng = lng;
	}
	public double getLat() {
		return lat;
	}
	public void setLat(double lat) {
		this.lat = lat;
	}
	
}
