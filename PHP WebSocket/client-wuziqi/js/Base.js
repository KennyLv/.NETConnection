// JavaScript Document
// 加载图片
function loadImage(srcList,callback){
	var imgs={};
	var totalCount=srcList.length;
	var loadedCount=0;
	for (var i=0;i<totalCount;i++){
		var img=srcList[i];
		var image=imgs[img.id]=new Image();		
		image.src=img.url;
		image.onload=function(event){
			loadedCount++;
		}		
	}
	if (typeof callback=="function"){
		var Me=this;
		function check(){
			if (loadedCount>=totalCount){
				callback.apply(Me,arguments);
			}else{		
				setTimeout(check,100);
			}	
		}
		check();
	}
	return imgs;
}

//有关 日期时间的几个函数
function GetDate()
{
   var d, s = "";           // 声明变量。
   d = new Date();                           // 创建 Date 对象。
   s += d.getFullYear() + "-";                         // 获取年份
   s += (d.getMonth() + 1) + "-";            // 获取月份。
   s += d.getDate() ;                   // 获取日。
   return(s);                                // 返回日期。
}

function GetTime()
{
   var d, s = "";           // 声明变量。
   d = new Date();                           // 创建 Date 对象。
   s += d.getHours() + ":";                         // 获取年份
   s += (d.getMinutes() ) + ":";            // 获取月份。
   s += d.getSeconds() + " ";                   // 获取日。
   return(s);                                // 返回日期。
}

function GetDateTime()
{
  return (GetDate()+" "+GetTime()) ;
}
