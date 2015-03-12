package com.webfin.websocket.dao;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.PreparedStatement;



import java.util.ArrayList;

import com.webfin.adv.entity.AdPara;
import com.webfin.common.dbo.DataBaseConnection;
import com.webfin.prod.dao.ProdListDao;
import com.webfin.vote.entity.ProdVote;
import com.webfin.websocket.entity.DeviceAccount;

public class DeviceAccountDao {


	/**
	 * 查询accountDevice是否存在
	 * 
	 * @return ArrayList
	 * @throws Exception
	 */
	public DeviceAccount getDaByNm(String deviceAccountNm,String type) {
		String str_sql = null;
		// 返回的结果List集合
		DeviceAccount retDa = null;
		Connection myconn = null;
		PreparedStatement  statement = null;
		ResultSet rs = null;
		DataBaseConnection dbc = null;

		try {
			dbc = new DataBaseConnection();
			myconn = dbc.getConn();
			str_sql = "SELECT * FROM DeviceAccount WHERE deviceNm= ? and type=? ";
			statement = myconn.prepareStatement(str_sql);
			statement.setString(1, deviceAccountNm);
			statement.setString(2, type);


			rs = statement.executeQuery();
			while (rs.next()) {
				retDa = new DeviceAccount();
				retDa.setId(rs.getInt("id"));
				retDa.setType(rs.getString("type"));
				retDa.setDeviceNm(rs.getString("devicenm"));
				retDa.setDeviceGroup(rs.getString("devicegroup"));
				retDa.setDevicePw(rs.getString("devicepw"));
				retDa.setLat(rs.getDouble("lat"));
				retDa.setLng(rs.getDouble("lng"));
				retDa.setStatus(rs.getString("status"));							
			}


		} catch (SQLException e) {
			System.out.println("DeviceAccountDao:getDaByNm error" + e.getMessage());
			return null;
		} catch (Exception e) {
			System.out.println("DeviceAccountDao:getDaByNm error" + e.getMessage());
			return null;
		} finally {
			try {
				rs.close();
				statement.close();
				myconn.close();

			} catch (SQLException e) {
				System.out.println("DeviceAccountDao:getDaByNm error"
						+ e.getMessage());
			}
		}
		return retDa;
	}
	
	
	/**
	 * 验证accountDevice
	 * 
	 * @return ArrayList
	 * @throws Exception
	 */
	public DeviceAccount verifyDa(String deviceAccountNm,String devicePin,String type) {
		String str_sql = null;
		// 返回的结果List集合
		DeviceAccount retDa = new DeviceAccount();
		retDa.setDeviceNm(deviceAccountNm);
		retDa.setDevicePw(devicePin);
		retDa.setType(type);
		Connection myconn = null;
		PreparedStatement  statement = null;
		ResultSet rs = null;
		DataBaseConnection dbc = null;

		try {
			dbc = new DataBaseConnection();
			myconn = dbc.getConn();
			str_sql = "SELECT * FROM DeviceAccount WHERE deviceNm= ? and devicepw = ? and type=? ";
			statement = myconn.prepareStatement(str_sql);
			statement.setString(1, deviceAccountNm);
			statement.setString(2, devicePin);
			statement.setString(3, type);


			rs = statement.executeQuery();
			while (rs.next()) {
				retDa.setId(rs.getInt("id"));
				retDa.setType(rs.getString("type"));
				retDa.setDeviceNm(rs.getString("devicenm"));
				retDa.setDeviceGroup(rs.getString("devicegroup"));
				retDa.setDevicePw(rs.getString("devicepw"));
				retDa.setLat(rs.getDouble("lat"));
				retDa.setLng(rs.getDouble("lng"));
				retDa.setStatus(rs.getString("status"));							
			}


		} catch (SQLException e) {
			System.out.println("DeviceAccountDao:getDaByNm error" + e.getMessage());
			return null;
		} catch (Exception e) {
			System.out.println("DeviceAccountDao:getDaByNm error" + e.getMessage());
			return null;
		} finally {
			try {
				rs.close();
				statement.close();
				myconn.close();

			} catch (SQLException e) {
				System.out.println("DeviceAccountDao:getDaByNm error"
						+ e.getMessage());
			}
		}
		return retDa;
	}



	/**
	 * 查询同一组accountDevice
	 * 
	 * @return ArrayList
	 * @throws Exception
	 */
	public ArrayList<DeviceAccount> getGroupDevices(DeviceAccount da) {
		String str_sql = null;
		// 返回的结果List集合
		ArrayList<DeviceAccount> retList = new ArrayList<DeviceAccount>();
		Connection myconn = null;
		PreparedStatement  statement = null;
		ResultSet rs = null;
		DataBaseConnection dbc = null;

		try {
			dbc = new DataBaseConnection();
			myconn = dbc.getConn();
			str_sql = "SELECT * FROM DeviceAccount WHERE deviceGroup= ? and ((type!='B' and status = 'repair') or (type='B' and status = 'online')) order by id desc";
			statement = myconn.prepareStatement(str_sql);
			statement.setString(1, da.getDeviceGroup());


			rs = statement.executeQuery();
			while (rs.next()) {
				DeviceAccount newDa = new DeviceAccount();
				newDa.setId(rs.getInt("id"));
				newDa.setType(rs.getString("type"));
				newDa.setDeviceNm(rs.getString("devicenm"));
				newDa.setDeviceGroup(rs.getString("deviceGroup"));
				newDa.setDevicePw(rs.getString("devicepw"));
				newDa.setLat(rs.getDouble("lat"));
				newDa.setLng(rs.getDouble("lng"));
				newDa.setStatus(rs.getString("status"));


				retList.add(newDa);
			}


		} catch (SQLException e) {
			System.out.println("DeviceAccountDao:getGroup error" + e.getMessage());
			return null;
		} catch (Exception e) {
			System.out.println("DeviceAccountDao:getGroup error" + e.getMessage());
			return null;
		} finally {
			try {
				rs.close();
				statement.close();
				myconn.close();

			} catch (SQLException e) {
				System.out.println("DeviceAccountDao:getGroup error"
						+ e.getMessage());
			}
		}
		return retList;
	}



	/**
	 * 查询accountDevice
	 * 
	 * @return ArrayList
	 * @throws Exception
	 */
	public ArrayList<DeviceAccount> getAll() {
		String str_sql = null;
		// 返回的结果List集合
		ArrayList<DeviceAccount> retList = new ArrayList<DeviceAccount>();
		Connection myconn = null;
		PreparedStatement  statement = null;
		ResultSet rs = null;
		DataBaseConnection dbc = null;

		try {
			dbc = new DataBaseConnection();
			myconn = dbc.getConn();
			str_sql = "SELECT * FROM DeviceAccount WHERE status != 'offline' order by id desc";
			statement = myconn.prepareStatement(str_sql);


			rs = statement.executeQuery();
			while (rs.next()) {
				DeviceAccount da = new DeviceAccount();
				da.setId(rs.getInt("id"));
				da.setType(rs.getString("type"));
				da.setDeviceNm(rs.getString("devicenm"));
				da.setDeviceGroup(rs.getString("devicegroup"));
				da.setDevicePw(rs.getString("devicepw"));
				da.setLat(rs.getDouble("lat"));
				da.setLng(rs.getDouble("lng"));
				da.setStatus(rs.getString("status"));


				retList.add(da);
			}


		} catch (SQLException e) {
			System.out.println("DeviceAccountDao:getAll error" + e.getMessage());
			return null;
		} catch (Exception e) {
			System.out.println("DeviceAccountDao:getAll error" + e.getMessage());
			return null;
		} finally {
			try {
				rs.close();
				statement.close();
				myconn.close();

			} catch (SQLException e) {
				System.out.println("DeviceAccountDao:getAll error"
						+ e.getMessage());
			}
		}
		return retList;
	}
	/**
	 * 插入设备信息
	 * 
	 * @param da
	 * @return boolean
	 * @throws Exception
	 */
	public boolean insertDevice(DeviceAccount da) {

		String str_sql = null;
		Connection myconn = null;
		PreparedStatement statement = null;
		DataBaseConnection dbc = null;


		try {
			dbc = new DataBaseConnection();
			myconn = dbc.getConn();


			str_sql = "INSERT INTO DeviceAccount (type,deviceGroup,deviceNm,devicePw,lng,lat,status) VALUES(?,?,?,?,?,?,?)";
			statement = myconn.prepareStatement(str_sql);
			statement.setString(1, da.getType());
			statement.setString(2, da.getDeviceGroup());
			statement.setString(3, da.getDeviceNm());
			statement.setString(4,da.getDevicePw());
			statement.setDouble(5,da.getLng());
			statement.setDouble(6,da.getLat());
			statement.setString(7,da.getStatus());

			if (statement.executeUpdate()>0) {
				statement.close();
				myconn.close();
			} else {
				return false;
			}
		} catch (SQLException e) {
			System.out.println("DeviceAccountDao:insert error" + e.getMessage());
			return false;
		} catch (Exception e) {
			System.out.println("DeviceAccountDao:insert error" + e.getMessage());
			return false;
		} finally {
			try {
				statement.close();
				myconn.close();

			} catch (SQLException e) {
				System.out
				.println("DeviceAccoutDao:insert error" + e.getMessage());
			}
		}
		return true;
	}	

	/**
	 * 计算目标设备信息
	 * 
	 * @param da
	 * @return boolean
	 * @throws Exception
	 */
	public DeviceAccount mathTargetDevice(DeviceAccount da) {

		String str_sql = null;
		Connection myconn = null;
		PreparedStatement statement = null;
		DataBaseConnection dbc = null;
		ResultSet rs = null;
		DeviceAccount retDa = null;

		try {
			dbc = new DataBaseConnection();
			myconn = dbc.getConn();


			str_sql = "select avg(lat) as lat,avg(lng) as lng from deviceAccount WHERE devicegroup = ? and ((type='A' and status='repair') or (type='B' and status='online'))";
			statement = myconn.prepareStatement(str_sql);
			statement.setString(1, da.getDeviceGroup());

			rs = statement.executeQuery();
			retDa = new DeviceAccount();
			while (rs.next()) {
				retDa.setLat(rs.getDouble("lat"));
				retDa.setLng(rs.getDouble("lng"));
				retDa.setType("B");
				retDa.setDeviceGroup(da.getDeviceGroup());
				retDa.setStatus("target");
				retDa.setDeviceNm("目标位置");			

			}
			statement.close();
			myconn.close();
			return retDa;

		} catch (SQLException e) {
			System.out.println("mathTargetDevice error" + e.getMessage());
			return null;
		} catch (Exception e) {
			System.out.println("mathTargetDevice error" + e.getMessage());
			return null;
		} finally {
			try {
				statement.close();
				myconn.close();

			} catch (SQLException e) {
				System.out
				.println("DeviceAccoutDao:update error" + e.getMessage());
			}
		}
	}	



	/**
	 * 更新设备信息
	 * 
	 * @param da
	 * @return boolean
	 * @throws Exception
	 */
	public boolean update(DeviceAccount da) {

		String str_sql = null;
		Connection myconn = null;
		PreparedStatement statement = null;
		DataBaseConnection dbc = null;

		try {
			dbc = new DataBaseConnection();
			myconn = dbc.getConn();


			str_sql = "UPDATE DeviceAccount set lat = ? ,lng=? , status = ? WHERE devicenm = ? and id = ?";
			statement = myconn.prepareStatement(str_sql);
			statement.setDouble(1, da.getLat());
			statement.setDouble(2, da.getLng());
			statement.setString(3, da.getStatus());
			statement.setString(4, da.getDeviceNm());
			statement.setInt(5,da.getId());

			if (statement.executeUpdate()>0) {
				statement.close();
				myconn.close();
			} else {
				return false;
			}
		} catch (SQLException e) {
			System.out.println("DeviceAccoutDao:update error" + e.getMessage());
			return false;
		} catch (Exception e) {
			System.out.println("DeviceAccoutDao:update error" + e.getMessage());
			return false;
		} finally {
			try {
				statement.close();
				myconn.close();

			} catch (SQLException e) {
				System.out
				.println("DeviceAccoutDao:update error" + e.getMessage());
			}
		}
		return true;
	}	


	/**
	 * 删除设备信息
	 * 
	 * @param prod_code
	 * @return boolean
	 * @throws Exception
	 */
	public boolean delete(int id) {

		String str_sql = null;
		Connection myconn = null;
		PreparedStatement statement = null;
		DataBaseConnection dbc = null;



		try {
			dbc = new DataBaseConnection();
			myconn = dbc.getConn();


			str_sql = "delete from DeviceAccount where id = ?";
			statement = myconn.prepareStatement(str_sql);
			statement.setInt(1, id);

			if (statement.executeUpdate()>0) {
				statement.close();
				myconn.close();
			} else {
				return false;
			}
		} catch (SQLException e) {
			System.out.println("DeviceAccountDao:delete error" + e.getMessage());
			return false;
		} catch (Exception e) {
			System.out.println("DeviceAccountDao:delete error" + e.getMessage());
			return false;
		} finally {
			try {
				statement.close();
				myconn.close();

			} catch (SQLException e) {
				System.out
				.println("DeviceAccountDao:delete error" + e.getMessage());
			}
		}
		return true;
	}


	public DeviceAccount getDaById(int deviceId) {
		String str_sql = null;
		// 返回的结果List集合
		DeviceAccount retDa = null;
		Connection myconn = null;
		PreparedStatement  statement = null;
		ResultSet rs = null;
		DataBaseConnection dbc = null;

		try {
			dbc = new DataBaseConnection();
			myconn = dbc.getConn();
			str_sql = "SELECT * FROM DeviceAccount WHERE id= ? ";
			statement = myconn.prepareStatement(str_sql);
			statement.setInt(1, deviceId);
			rs = statement.executeQuery();
			while (rs.next()) {
				retDa = new DeviceAccount();
				retDa.setId(rs.getInt("id"));
				retDa.setType(rs.getString("type"));
				retDa.setDeviceNm(rs.getString("devicenm"));
				retDa.setDeviceGroup(rs.getString("devicegroup"));
				retDa.setDevicePw(rs.getString("devicepw"));
				retDa.setLat(rs.getDouble("lat"));
				retDa.setLng(rs.getDouble("lng"));
				retDa.setStatus(rs.getString("status"));							
			}


		} catch (SQLException e) {
			System.out.println("DeviceAccountDao:getDaById error" + e.getMessage());
			return null;
		} catch (Exception e) {
			System.out.println("DeviceAccountDao:getDaById error" + e.getMessage());
			return null;
		} finally {
			try {
				rs.close();
				statement.close();
				myconn.close();

			} catch (SQLException e) {
				System.out.println("DeviceAccountDao:getDaById error"
						+ e.getMessage());
			}
		}
		return retDa;

	}
}
