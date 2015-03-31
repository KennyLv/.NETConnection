// JavaScript Document

function game( canvas , posCanvas , image_cash , ws  )
{
	var mainCanvas = canvas ;  //主canvas
	var posCanvas  = posCanvas ; //显示棋子位置的 canvas
	var mainCt     = mainCanvas.getContext('2d') ;
	//var posCt      = posCanvas.getContext('2d') ;
	var WSobj = null ;
	var SocketState = 0 ;
	
	var timer = null ; //计时器
	var  that = this ;
	
	var time_total = 600 ;
	var m_spare_time = 600 ;  //开始 剩余时间 为 600 s
	var a_spare_time = 600 ;
	
	//棋盘 其实位置
	var QiPan_left = 150 ;
	var QiPan_top  = 30  ;
	var QiPan_width  = 30 ;
	var imgHalfWidth = 12 ;  //棋子图片 半宽度
	
	var img = image_cash ;  //已经加载到内存中的图片对象
	
	var game_data = null ;  //15*15数组，保存棋盘上的棋子信息 0-空 1-黑棋 2-白棋
	var pos_x = [] ;
	var pos_y = [] ;
	
	var m_score = 0 ;  //我发比分
	var a_score = 0 ;  //对家比分
	var m_prepared = false ; //我方是否准备v
	var a_prepared = false ; //对方是否准备v
	
	var m_player   = '' ;  //己方玩家登录名
	var a_player = '' ;   //对方玩家登录名
	var gameState = 0 ; //游戏状态 0-初始化状态 1-正在游戏中 2-有玩家未准备
	var m_QiZiType = 0 ;  //我方落子类型 1-黑棋 2-白棋
	var run_palyer = 0 ;  //现行方  0-初始化状态 1-允许我方落子 2-允许对方洛子
	
	//设置 玩家棋子类型
	this.setQiZiType = function( type )
	{
		if( type>=1 && type<=2 )
		{
			m_QiZiType = type ;
		}
	}
	//设置 现行方
	this.setRunPlayer=function( player )
	{
		if( player>=1 && player<=2 )
		{
			run_player = player ;
		}
	}
	this.setPlayer=function( name )
	{
		m_player = name ;
		that.setBtnState();
	}
	this.setAnoPlayer = function( name )
	{
		a_player = name ;
		that.setBtnState();
	}
	this.setGameState= function( state )
	{
		if( state>=0 && state<=2 )
		{
			gameState = state ;
			that.setBtnState();
		}
		//向服务器更新自己的游戏状态
		if( parseInt(SocketState)==1 )
		{
			WSobj.send(JSON.stringify({
                        type:'updateState',
                        //name:js.dom.$('AnotherUserName').innerHTML,
						state:state
                    }));
		}
	}
	this.setSocket= function( ws )
	{
		WSobj = ws ;
	}
	this.setSocketState= function( state )
	{
		SocketState = state ;
		that.setBtnState();
	}
	this.setMyPrepared = function( ifPrepared )
	{
		if(  ifPrepared==true ||  ifPrepared==false  )
		{
			m_prepared = ifPrepared ;
			that.setBtnState();
		}
	}
	this.getSocketState= function()
	{
		return SocketState ;
	}
	this.getQiZiType = function()
	{
		return m_QiZiType ;
	}
	this.getPlayer = function()
	{
		return m_player ;
	}
	this.getAnoPlayer = function()
	{
		return a_player ;
	}
	//设置按钮的状态
	this.setBtnState = function()
	{
		//刷新按钮
		if( parseInt(SocketState)==1  )
		{
			js.dom.$('btnRefresh').disabled = false ;
		}else
		{
			js.dom.$('btnRefresh').disabled = true ;
		}
		//连接用户 按钮  与 注销 按钮
		if( parseInt(SocketState)==1  &&  m_player!='' && a_player==''  )
		{
			showConnectUser(true);
			js.dom.$('btnConnect').disabled = false ;
			js.dom.$('btnLogout').disabled = false ;
		}else
		{
			//showConnectUser(false);
			js.dom.$('btnConnect').disabled = true ;
			js.dom.$('btnLogout').disabled = true ;
		}
		//开始按钮
		if( parseInt(SocketState)==1  &&  m_player!='' && a_player!='' && parseInt(gameState)==2 && m_prepared==false )
		{
			js.dom.$('btnStart').disabled = false ;
		}else
		{
			js.dom.$('btnStart').disabled = true ;
		}
		//和棋、悔棋、认输 按钮
		if(  parseInt(SocketState)==1  &&  m_player!='' && a_player!='' && parseInt(gameState)==1  )
		{
			//if( pos_x.length>=2 && pos_y.length>=2 )  js.dom.$('btnRegret').disabled = false ;
//			else js.dom.$('btnRegret').disabled = true ;
			js.dom.$('btnRegret').disabled = false ;
			js.dom.$('btnDraw').disabled = false ;
			js.dom.$('btnGiveUp').disabled = false ;
		}else
		{
			js.dom.$('btnRegret').disabled = true ;
			js.dom.$('btnDraw').disabled = true ;
			js.dom.$('btnGiveUp').disabled = true ;
		}
		
		//退出(离开房间) 按钮
		if(  parseInt(SocketState)==1  &&  m_player!='' && a_player!='' && parseInt(gameState)==2 )
		{
			js.dom.$('btnExit').disabled = false ;
		}else
		{
			js.dom.$('btnExit').disabled = true ;
		}
		//发送 聊天信息的按钮
		if(  parseInt(SocketState)==1  &&  m_player!='' && a_player!='' && parseInt(gameState)>=1 )
		{
			js.dom.$('btnSend').disabled = false ;
		}else
		{
			js.dom.$('btnSend').disabled = true ;
		}
		
	}
	//有个get的函数
	
	this.init = function()
	{
		if( window.addEventListener )  //对于火狐和谷歌内核的浏览器 使用一下函数关联鼠标事件
		{
			mainCanvas.addEventListener( 'mousedown' ,mousedownHandle , false  );
			mainCanvas.addEventListener( 'mousemove' ,mousemoveHandle , false  );
			mainCanvas.addEventListener( 'mouseout' ,mouseoutHandle , false  );
		}else if ( window.attachEvent )//对于IE内核浏览器 使用一下函数关联鼠标事件
		{
			mainCanvas.attachEvent( 'mousedown' ,mousedownHandle  );
			mainCanvas.attachEvent( 'mousemove' ,mousemoveHandle  );
			mainCanvas.attachEvent( 'mouseout' ,mouseoutHandle  );
		}
		startData();
		drawQiPan();
		drawInfos();
	}

	var startData = function()
	{
		if(game_data==null )
		{
			game_data = new Array();
			for( var i=0 ; i<15 ; i++)
			{
				var temp = new Array();
				for( var j=0 ;j<15 ; j++  )
				{
					temp.push( 0 );
				}
				game_data.push( temp );
			}
		}else
		{
			for( var i=0 ; i<15 ; i++)
			{
				for( var j=0 ;j<15 ; j++  )
				game_data[i][j] = 0 ; 
			}
		}
		//初始化其他数据
		pos_x = [] ;
	    pos_y = [] ;
		that.setMyPrepared( false );
		a_prepared = false ;
		m_spare_time = 600 ;
		a_spare_time = 600
		
	}
	//start 事件处理
	var mousedownHandle = function()
	{
		//alert('mouse down');
		if( run_palyer!=1 || gameState!=1 )
		{
			mainCanvas.style.cursor = 'default' ;
			return ;
		}
		var evt= window.event || arguments[0] ;
		if(evt.offsetX && evt.offsetY )
		{
			x=evt.offsetX ;
			y=evt.offsetY ;
		}else{
		var box = (evt.target || evt.srcElement).getBoundingClientRect(), 
		 x = parseInt( evt.clientX - box.left ) ;
		 y = parseInt( evt.clientY - box.top ); 
		} 
		//alert( x + "," + y );
		if( x<QiPan_left || x>(   parseInt(QiPan_left)+QiPan_width*14 ) || y<QiPan_top || y>( parseInt(QiPan_top)+QiPan_width*14 ) )
		{
			mainCanvas.style.cursor = 'default' ;
			return ;
		}
		var lines = 0 ;
		var rows = 0 ;
		lines = parseInt( ( x - QiPan_left )/QiPan_width );
		lines_spare = ( x - QiPan_left )%QiPan_width ;
		if( lines_spare>( QiPan_width/2  ) ) lines += 1 ;
		rows = parseInt( ( y - QiPan_top )/QiPan_width );
		rows_spare = ( y- QiPan_top )%QiPan_width ;
		if( rows_spare>( QiPan_width/2  ) ) rows += 1 ;
		if( game_data[rows][lines]>0 )
		{
			return ;
		}
		//alert( lines +"," +rows );
		forwardData( 'GamePos_' + lines +"," +rows);//通过服务器将自己的棋子信息发给对家
		game_data[rows][lines] = m_QiZiType ;
		pos_x.push( rows );
		pos_y.push( lines );
		run_palyer = 3 - run_palyer ;//转换现行方 
		drawQiZi(m_QiZiType ,   lines ,rows  );//绘制刚下棋子
		var result = checkWin( game_data );
		//alert( result );
		if( result==1 )
		{
			that.finishGame();
			alert( '五星连珠，黑胜！' );
		}else if( result==2 )
		{
			that.finishGame();
			alert( '五星连珠，白胜！' );
		}
		if( result>=1  )
		{
			if( result==m_QiZiType )
			{
				m_score += 1 ;
			}else if( result==3-m_QiZiType )
			{
				a_score += 1 ;
			}
		}
	}
	var mousemoveHandle = function()
	{
		//alert('mouse move');
		if( run_palyer!=1 || gameState!=1 )
		{
			mainCanvas.style.cursor = 'default' ;
			return ;
		}
		var evt= window.event || arguments[0] ;
		if(evt.offsetX && evt.offsetY )
		{
			x=evt.offsetX ;
			y=evt.offsetY ;
		}else{
		var box = (evt.target || evt.srcElement).getBoundingClientRect(), 
		 x = parseInt( evt.clientX - box.left ) ;
		 y = parseInt( evt.clientY - box.top ); 
		} 
		//alert( x + "," + y );
		if( x<QiPan_left || x>(   parseInt(QiPan_left)+QiPan_width*14 ) || y<QiPan_top || y>( parseInt(QiPan_top)+QiPan_width*14 ) )
		{
			mainCanvas.style.cursor = 'default' ;
			return ;
		}
		var lines = 0 ;
		var rows = 0 ;
		lines = parseInt( ( x - QiPan_left )/QiPan_width );
		lines_spare = ( x - QiPan_left )%QiPan_width ;
		if( lines_spare>( QiPan_width/2  ) ) lines += 1 ;
		rows = parseInt( ( y - QiPan_top )/QiPan_width );
		rows_spare = ( y- QiPan_top )%QiPan_width ;
		if( rows_spare>( QiPan_width/2  ) ) rows += 1 ;
		if( game_data[rows][lines]>0 )
		{
			mainCanvas.style.cursor = 'default' ;
			return ;
		}else
		{
			mainCanvas.style.cursor = 'pointer' ;
		}
	}
	var mouseoutHandle = function()
	{
		mainCanvas.style.cursor = 'default' ;
	}
	//end 事件处理

    //有关 界面绚烂 方面的函数
	var drawQiPan = function()
	{
		mainCt.clearRect( QiPan_left - QiPan_width/2 , QiPan_top - QiPan_width/2 , 14*QiPan_width + QiPan_width/2 , 14*QiPan_width + QiPan_width/2 );
		mainCt.beginPath();
		for( var i=0 ; i<15 ; i++ )
		{
			//画横线
			mainCt.moveTo( QiPan_left , QiPan_top + i*QiPan_width );
			mainCt.lineTo( QiPan_left + 14*QiPan_width , QiPan_top + i*QiPan_width );
			//画竖线
			mainCt.moveTo( QiPan_left + i*QiPan_width , QiPan_top );
			mainCt.lineTo( QiPan_left + i*QiPan_width , QiPan_top + 14*QiPan_width );
		}
		mainCt.stroke();
		mainCt.closePath();
	}
	//绘制 单个棋子
	var drawQiZi = function( type , x , y )
	{
		var imgId = '' ;
		if( type==1 )
		{
			imgId = 'heiqi' ;
		}else if( type==2 )
		{
			imgId = 'baiqi' ;
		}
		mainCt.drawImage( img[imgId]  ,QiPan_left + QiPan_width*x-imgHalfWidth ,QiPan_top + QiPan_width*y-imgHalfWidth  );
	}
	//绘制所有棋子 包括空棋盘
	var drawAllQiZi = function()
	{
		drawQiPan();
		for( i=0 ; i<pos_x.length;i++ )
		{
			var rows = pos_x[i] ;
			var lines = pos_y[i] ;
			drawQiZi( game_data[rows][lines] , lines , rows );
		}
	}

	var drawInfos = function()
	{
		mainCt.clearRect( 20 , 100 , 114 , 300 );
		drawText( 20 , 50 , '己方：'+m_player );
		drawText( 20 , 100 , '总时间：'+changeToStr(time_total) );
		drawText( 20 , 150 , '倒计时：'+changeToStr(m_spare_time) );
		
		drawText( 20 , 250 , '对方：'+a_player );
		drawText( 20 , 300 , '总时间：'+changeToStr(time_total) );
		drawText( 20 , 350 , '倒计时：'+changeToStr(a_spare_time) );
		if( m_QiZiType==1 )
		{
			mainCt.drawImage( img['heiqi'] , 50 , 190 );
			mainCt.drawImage( img['baiqi'] , 50 , 390 );
		}else
		{
			mainCt.drawImage( img['heiqi'] , 50 , 390 );
			mainCt.drawImage( img['baiqi'] , 50 , 190 );
		}
	}
	//绘制文字
	var drawText = function( left , top , text , align )
	{
		mainCt.textBaseline = 'top' ;
		if( typeof(align)=='undefined' )
		{
			mainCt.textAlign = 'left' ;
		}else
		{
			mainCt.textAlign = align ;
		}
		mainCt.font = "12px sans-serif" ;
		mainCt.fillText( text , left , top );
	}
	
	//有关程序逻辑控制的函数
	this.StartGame = function()
	{
		if( m_QiZiType==1 )
		{
			run_palyer = 1 ;
		}else if( m_QiZiType==2 )
		{
			run_palyer = 2 ;
		}
		that.setGameState(1);
		timer = setInterval( updateTime, 1000 );
	}
	var updateTime=function()
	{
		if( m_spare_time<=0 )
		{
			that.finishGame();
			alert('你已经超时！判负！');
			return ;
		}
		if( a_spare_time<=0  )
		{
			that.finishGame();
			alert('对方已经超时！判负！');
			return ;
		}
		if( run_palyer==1 )
		{
			m_spare_time -= 1 ;
		}else if( run_palyer==2 )
		{
			a_spare_time -= 1 ;
		}
		drawInfos();
	}
	this.finishGame= function()
	{
		//销毁 掉时钟
		if( timer!=null )
		{
			clearInterval( timer );
		}
		startData();
		m_QiZiType = 3- m_QiZiType ;  //交换棋子类型
		that.setGameState(2);
	}
	//检测 是否有一方 胜利 return 0-无 1-黑子胜利 2-白棋胜利
	var checkWin = function()
	//function checkWin( game_data )
	{
		//检测 横竖 时候有五星连珠
		for( var i=0 ; i<=10 ; i++ )
		{
			for( var j=0 ; j<=10 ; j++ )
			{
				if(  game_data[i][j]==1 && game_data[i+1][j]==1 && game_data[i+2][j]==1 && game_data[i+3][j]==1 && game_data[i+4][j]==1 )
				{
					return 1 ;
				}
				if(  game_data[i][j+1]==1 && game_data[i][j+2]==1 && game_data[i][j+3]==1 && game_data[i][j+4]==1 && game_data[i][j]==1 )
				{
					return 1 ;
				}
				if(  game_data[i][j]==2 && game_data[i+1][j]==2 && game_data[i+2][j]==2 && game_data[i+3][j]==2 && game_data[i+4][j]==2 )
				{
					return 2 ;
				}
				if(  game_data[i][j+1]==2 && game_data[i][j+2]==2 && game_data[i][j+3]==2 && game_data[i][j+4]==2 && game_data[i][j]==2 )
				{
					return 2 ;
				}
			}
		}
		//检测 斜的方向 时候 有五星连珠
		for( var i=0 ; i<=14; i++ )
		{
			for( var j=0 ; j<=14 ; j++ )
			{
				if(  i<=10 && j>=4 &&  game_data[i][j]==1 && game_data[i+1][j-1]==1 && game_data[i+2][j-2]==1 && game_data[i+3][j-3]==1 && game_data[i+4][j-4]==1 )
				{
					return 1 ;
				}
				if(  i<=10 && j<=10 && game_data[i][j]==1 && game_data[i+1][j+1]==1 && game_data[i+2][j+2]==1 && game_data[i+3][j+3]==1 && game_data[i+4][j+4]==1 )
				{
					return 1 ;
				}
				if(  i<=10 && j>=4 &&  game_data[i][j]==2 && game_data[i+1][j-1]==2 && game_data[i+2][j-2]==2 && game_data[i+3][j-3]==2 && game_data[i+4][j-4]==2 )
				{
					return 2 ;
				}
				if(  i<=10 && j<=10 && game_data[i][j]==2 && game_data[i+1][j+1]==2 && game_data[i+2][j+2]==2 && game_data[i+3][j+3]==2 && game_data[i+4][j+4]==2 )
				{
					return 2 ;
				}
			}
		}
		return 0 ;
	}
	//当点击 了开始按钮后 响应该函数
	this.start= function()
	{
		if( gameState!=2 ) return ;
		forwardData( 'GameData_start' ) ;
		that.setMyPrepared( true );
		drawQiPan();
		drawInfos();
		if( a_prepared && m_prepared )  //两位玩家都已经准备好
		{
			this.StartGame();
		}
	}
	this.giveUp = function()  //认输
	{
		forwardData( 'GameData_giveUp' ) ;
		that.finishGame();
	}
	//退出
	this.exit = function()
	{
		forwardData( 'GameData_exit' ) ;
		that.finishGame();
		that.setAnoPlayer('');
	}
	//悔棋
	this.RegretPlease = function()
	{
		if( pos_x.length>=2 && pos_y.length>=2 )
		{
			forwardData('GameData_regret')
		}else
		{
			alert('落子数不足，目前还不能悔棋!');
		}
	}
	//悔棋 操作
	var Regret = function( ifMeRegret )
	{
		var lastX = pos_x[ pos_x.length -1 ];
		var lastX2= pos_x[ pos_x.length -2 ];
		var lastY = pos_y[ pos_y.length -1 ];
		var lastY2= pos_y[ pos_y.length -2 ];
		game_data[lastX][lastY] = 0 ;
		pos_x.pop();
		pos_y.pop();
		if( (ifMeRegret && run_palyer==1) || (  !ifMeRegret && run_palyer==2 )  )
		{
			game_data[lastX2][lastY2] = 0 ;
			pos_x.pop();
			pos_y.pop();
		}else
		{
			run_palyer = 3 - run_palyer ;
		}
		drawAllQiZi();
	}
	//将秒数 转换为 "mm:SS"格式的字符串
	var changeToStr =function( s )
	{
		if( s<0 || s>time_total ) return  ;
		var mm = parseInt( s / 60 ) ;
		var ss  = parseInt( s % 60 ) ;
		if( mm<10 ) mm = "0" + mm ;
		if( ss<10 ) ss = "0" + ss ;
		var time = mm + ":" + ss ;
		return time ;
	}
	//与webSocket有关的函数
	var forwardData=function( msg )
	{
		if( a_player=='' )
		{
			alert('你还未连接上用户！');
			return ;
		}
		WSobj.send(JSON.stringify({
                        type:'forward',
                        name:a_player,
						msg:msg
                    }));
	}
	//处理 收到的 棋子位置信息
	this.GamePosDataHandle = function( data )
	{
		var lines = data.split(',')[0] ;
		var rows  = data.split(',')[1] ;
		run_palyer = 3 - run_palyer ;
		//alert( rows +"," + lines );
		game_data[rows][lines] = 3-m_QiZiType ;
		pos_x.push( rows );
		pos_y.push( lines );
		drawQiZi(3-m_QiZiType , lines ,  rows );
		var result = checkWin( game_data );
		//alert( result );
		if( result==1 )
		{
			that.finishGame();
			alert( '五星连珠，黑胜！' );
		}else if( result==2 )
		{
			that.finishGame();
			alert( '五星连珠，白胜！' );
		}
		if( result>=1  )
		{
			if( result==m_QiZiType )
			{
				m_score += 1 ;
			}else if( result==3-m_QiZiType )
			{
				a_score += 1 ;
			}
		}
	}
	//处理收到的 游戏信息
	this.GameDataHandle = function( data )
	{
		switch( data )
		{
			case 'start':
			a_prepared = true ;
			if( a_prepared && m_prepared )  //两位玩家都已经准备好
			{
				this.StartGame();
			}
			break;
			case 'draw': //和棋
			if( confirm('对方请求和棋，你是否同意？') )
			{
				forwardData( 'GameData_AccDraw' ) ;
				that.finishGame();
			}else
			{
				forwardData( 'GameData_RejDraw' ) ;
			}
			break;
			case 'regret':  //悔棋
			if( confirm('对方请求悔棋，你是否同意?') )
			{
				forwardData( 'GameData_AccRegret' ) ;
				Regret( false );
			}else
			{
				forwardData( 'GameData_RejRegret' ) ;
			}
			break;
			case 'giveUp':  //认输
			alert('对方已认输,本局游戏结束!');
			that.finishGame();
			break;
			case 'exit':  //退出（离开房间）
			that.setAnoPlayer('');
			that.finishGame();
			break;
			case 'AccDraw':  //同意和棋
			alert('对方同意和棋，本局游戏结束!');
			that.finishGame();
			break;
			case 'RejDraw': //拒绝和棋
			alert('对方拒绝了和棋！');
			break;
			case 'AccRegret': //同意悔棋
			alert('对方同意了你的悔棋请求!');
			Regret( true );
			break;
			case 'RejRegret': //拒绝悔棋
			alert('对方拒绝了悔棋！');
			break;
		}
	}
}

