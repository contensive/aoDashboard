
//
// -- event to resize dash
function dashResize() {
	console.log("dashResize");
	var i,n,nodeTop,nodeBottom,desktopBottom=0;
	jQuery(".dashNode").each(function(){
		n=document.getElementById(this.id)
		if(n){
			console.log("dashResize, initialize node["+this.id+"]");
			nodeTop=n.style.top;
			nodeTop=parseInt(nodeTop.replace("px",""));
			nodeBottom=nodeTop+n.scrollHeight;
			if(nodeBottom>desktopBottom){desktopBottom=nodeBottom}
		}
	})
	if(desktopBottom!=0){
		n=document.getElementById("dashBoardWrapper");
		if(n){n.style.height=desktopBottom}
	}
}
//
// -- mouse over body
function overBody(toolsId){
	e=document.getElementById(toolsId);
	showTools(e);
	e.overBody=true;
	if(e.t) clearTimeout(e.t);
}
//
// -- mouse out of body
function outBody(toolsId){
	e=document.getElementById(toolsId);
	e.overBody=false;
	if(e.oTools){
		if(e.oTools==false){
			e.t=setTimeout("hideTools(\""+e.id+"\")",3000);
		}
	} else {
		e.t=setTimeout("hideTools(\""+e.id+"\")",3000);
	}
}
function overTools(e){
	showTools(e);
	e.oTools=true;
	if(e.t) clearTimeout(e.t);
}
function outTools(e){
	e.oTools=false;
	if(e.overBody){
		if(e.overBody==false){
			e.t=setTimeout("hideTools(\""+e.id+"\")",3000);	
		}
	} else {
		e.t=setTimeout("hideTools(\""+e.id+"\")",3000);	
	}
}
function showTools(e){
	if(e){
		if(e.style.visibility!="visible"){
			e.style.visibility="visible";
		}
	}
}
function hideTools(toolsId){
	var e=document.getElementById(toolsId);
	if(e) e.style.visibility="hidden";
}
function dashDeleteNode( nodePtr, htmlId ) {
	var c=document.getElementById(htmlId);
	var p=c.parentNode;
	p.removeChild(c);
	cj.ajax.addon("dashboarddelnode","key="+nodePtr);
	dashResize();
}
function dashOpenNodeCallback(response,callbackArg) {
	var hId="dashhelper"+nodeCnt++;
	navMakeHelper(hId);
	var e=document.getElementById(hId);
	e.innerHTML=response;
	dashResize();
}
function dashOpenNode( nodePtr, htmlId ) {
//alert("dashOpenNode");
	var c,p,hId;
	c=document.getElementById(htmlId);
	p=c.parentNode;
	p.removeChild(c);
	cj.ajax.addonCallback("dashboardopennode","key="+nodePtr,dashOpenNodeCallback,"test");
}
function closeNode( nodePtr, htmlId ) {
	var c,p,hId;
	c=document.getElementById(htmlId);
	var p=c.parentNode;
	p.removeChild(c);
	hId="dashhelper"+nodeCnt++;
	navMakeHelper(hId);
	cj.ajax.addon("dashboardclosenode","key="+nodePtr,"",hId);
	dashResize();
}
function navMakeHelper( iconId ) {
	var w=document.getElementById("dashBoardWrapper");
	var e=document.createElement("div");
	e.setAttribute("id",iconId);
	e.setAttribute("style","position:absolute;top:0;left:0;");
	w.appendChild(e);
}
function navDropCallback(response,iconId){
	var e=document.getElementById(iconId);
	e.innerHTML=response;
	dashResize();
}
function navDrop(id,x,y){
	console.log("navDrop");
	var iconId="dashhelper"+nodeCnt++;
	console.log("navDrop, iconId ["+iconId+"]");
	var w=document.getElementById("dashBoardWrapper");
	var e=document.createElement("div");
	var posX=(x-getPageOffsetLeft(w));
	var posY=(y-37-getPageOffsetTop(w));
	console.log("navDrop, posX ["+posX+"], posY ["+posY+"]");
	e.setAttribute("id",iconId);
	e.setAttribute("style","position:absolute;top:0;left:0;");
	w.appendChild(e);
	var qs = "id="+id+"&x="+posX+"&y="+posY;
	console.log("navDrop, qs ["+qs+"]");
	cj.ajax.addonCallback("dashboardnavdrop",qs,navDropCallback,iconId);
	// cj.ajax.addon("dashboardnavdrop","id="+id+"&x="+posX+"&y="+posY,"",iconId);
	// dashResize();
}
/*
* OnReady
*/
jQuery( document ).ready(function(){
	/*
	* make entire dashboard droppable
	*/	
	jQuery("#desktop").droppable({tolerance: "fit"});
	/*
	* bind to icon nodes
	*/	
	jQuery(".dashNode").each(function(){
		jQuery(this).draggable({
			stop: function(event, ui){
				var qs="key="+this.id+"&x="+this.style.left+"&y="+this.style.top;
				console.log("draggable:stop, qs ["+qs+"]");
				cj.ajax.addonCallback("dashboarddragstop",qs,dashResize);
			}
			,start: function(event, ui){
				this.style.zIndex=iconZIndexTop++;
				jQuery(this).draggable("option", "zIndex", iconZIndexTop );
			}
			,revert: "invalid"
			,zIndex: 0
			,hoverClass: "droppableHover"
			,opacity: 0.50
			,handle: "#toolBar"+this.id
			,cursor: "move"
		});
	});	
	/*
	* bind resizable
	*/
	jQuery(".windowNode").each(function(){
		jQuery(this).resizable({
			alsoResize: "#designResizer"+this.id
			,stop: function(event, ui) {
				var r=document.getElementById("designResizer"+this.id);
				cj.ajax.addon("dashboardresize","key="+this.id+"&x="+this.style.width+"&y="+r.style.height);
				dashResize();
			}
		});
	});	
	/*
	* Initialize
	*/
	dashResize();
});