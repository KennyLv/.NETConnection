// JavaScript Document
//处理 收到 “用户请求加入游戏”
function joinGameDataHandle( data )
{
	if( confirm( data+'请求和你玩游戏，你是否同意?') )  //同意对方请求
	{
		js.dom.$('AnotherUser').value = data ;
		js.dom.$('AnotherUserName').innerHTML = data ;
		js.dom.$('divConnect').style.display = 'none' ;
		js.dom.$('divConnectSuccess').style.display = 'block' ;
		WSobj.send(JSON.stringify({
                        type:'forward',
                        name:data,
						msg:'AccGame_'+js.dom.$('name').value
                    }));
		//设置对家 玩家的姓名
		g_game.setAnoPlayer( data );
		g_game.setGameState( 2 ) ;
		g_game.setQiZiType( 1 );
	}else  //拒绝对方请求
	{
		WSobj.send(JSON.stringify({
                        type:'forward',
                        name:data,
						msg:'RejGame_'+js.dom.$('name').value
                    }));
	}
}
//用户登录信息 处理
function loginDataHandle(  data )
{
   if( data=='Accept' )
   {
	  js.dom.$('divLoginSuccess').style.display = 'block' ;
	  js.dom.$('divLogin').style.display = 'none' ;
	  js.dom.$('divHint').style.display = 'none' ;
	  //设置 自己玩家姓名
	  g_game.setPlayer( js.dom.$('name').value );
	  showLog( '服务端','你已成功登录' ) ;
   }else
   {
	  alert( data );
   }
}
//用户注销
function logoutDataHandle( data )
{
   var div = js.dom.$('userInfos') ;
   div.removeChild( js.dom.$('div_'+data) );
}
//用户 离开 处理
function exitDataHandle( data )
{
   var div = js.dom.$('userInfos') ;
   div.removeChild( js.dom.$('div_'+data) );
   showLog( '服务端','玩家'+data+'离开了' ) ;
   if( g_game.getAnoPlayer()==data )  //如果 离开的用户 恰好是 对家玩家  则表示 对方已经掉线!!
   {
	   alert('抱歉，对家掉线，请重新找一位玩家！');
	   g_game.finishGame();
	   g_game.setGameState(0);
	   g_game.setAnoPlayer('');
	   g_game.setMyPrepared(false);
   }
   
}
//用户进入
function UserAddDataHandle( data )
{
   var div = js.dom.$('userInfos') ;
   var divId = "div_"+data ;
   var divUserHTML = "<div  style='width:50%; float:left'>"+data+"</div>";
   var divStateHTML = "<div style='width:50%; float:right; color:#FF0000;'><input type='hidden' value='0'><span>等待中</span></div>" ;
   div.innerHTML += "<div id='"+divId+"'  style='width:100%;' onClick='changeBG( this.parentNode.id , this.id )'>"+divUserHTML+divStateHTML+"</div>" ;
   showLog( '服务端','玩家'+data+'登录了服务器' ) ;
}
//用户状态 更新
function UpdateStateHandle( data )
{
	var data_arr = data.split(':::') ;
	var username = data_arr[0] ;
	var state    = data_arr[1] ;
	var div = js.dom.$('div_'+username) ;
	var hided_obj = div.getElementsByTagName('input')[0];
	var span   = div.getElementsByTagName('span')[0];
	hided_obj.value = state ;
	switch( parseInt(state) )
	{
		case 0:
		span.innerHTML = '等待中' ;
		break;
		case 1:
		span.innerHTML = '游戏中' ;
		break;
		case 2:
		span.innerHTML = '准备中' ;
		break;
		default:
		break;
	}
}
//所有登陆用户信息
function UserInfosDataHandle( data )
{
   var div = js.dom.$('userInfos') ;
   div.innerHTML = "<div style='width:100%; text-align:left'><div  style='width:50%; float:left'>玩家名称</div><div style='width:50%; float:right;'>游戏状态</div></div>" ;
   var users = data.split(",") ;
   //alert( data );
   //alert( users.length );
   for( i=0 ; i<users.length ; i++ )
   {
	   var username = users[i].split(':::')[0] ;
	   var state = users[i].split(':::')[1] ;
	   var divId = "div_"+username ;
	  if( username=='' )  continue ;
	  switch( parseInt(state) )
		{
			case 0:
			spanHTML = '等待中' ;
			break;
			case 1:
			spanHTML = '游戏中' ;
			break;
			case 2:
			spanHTML = '准备中' ;
			break;
			default:
			break;
		}
	  var divUserHTML = "<div  style='width:50%; float:left'>"+username+"</div>";
      var divStateHTML = "<div style='width:50%; float:right; color:#FF0000;'><input type='hidden' value='"+state+"'><span>"+spanHTML+"</span></div>" ;
      div.innerHTML += "<div id='"+divId+"'  style='width:100%;' onClick='changeBG( this.parentNode.id , this.id )'>"+divUserHTML+divStateHTML+"</div>" ;
	  //div.innerHTML += "<div id='div_"+users[i]+"'  style='width:100%;' onClick='changeBG( this.parentNode.id , this.id )'>"+users[i]+"</div>" ;
   }
}