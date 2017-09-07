//
// -- initialize
jQuery(document).ready(function(){
	dashResize();
})
//
// -- event to resize dash
function dashResize() {
	console.log('dashResize, nodeCnt['+nodeCnt+']');
	var i,n,nodeTop,nodeBottom,desktopBottom=0;
	for (i=0;i<=nodeCnt;i++){
		n=document.getElementById('dashnode'+i)
		if(n){
			console.log('dashResize, initialize node['+i+']');
			nodeTop=n.style.top;
			nodeTop=parseInt(nodeTop.replace('px',''));
			nodeBottom=nodeTop+n.scrollHeight;
			if(nodeBottom>desktopBottom){desktopBottom=nodeBottom}
		}
	}
	if(desktopBottom!=0){
		n=document.getElementById('dashBoardWrapper');
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
			e.t=setTimeout("hideTools('"+e.id+"')",3000);
		}
	} else {
		e.t=setTimeout("hideTools('"+e.id+"')",3000);
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
			e.t=setTimeout("hideTools('"+e.id+"')",3000);	
		}
	} else {
		e.t=setTimeout("hideTools('"+e.id+"')",3000);	
	}
}
function showTools(e){
	if(e){
		if(e.style.visibility!='visible'){
			e.style.visibility='visible';
		}
	}
}
function hideTools(toolsId){
	var e=document.getElementById(toolsId);
	if(e) e.style.visibility='hidden';
}
function dashDeleteNode( nodePtr, HTMLId ) {
	var c=document.getElementById(HTMLId);
	var p=c.parentNode;
	p.removeChild(c);
	cj.ajax.addon('dashboarddelnode','ptr='+nodePtr);
	dashResize();
}
function dashOpenNodeCallback(response,callbackArg) {
//alert('dashOpenNodeCallback');
	var hId='dashhelper'+nodeCnt++;
	navMakeHelper(hId);
	var e=document.getElementById(hId);
	e.innerHTML=response;
	dashResize();
}
function dashOpenNode( nodePtr, HTMLId ) {
//alert('dashOpenNode');
	var c,p,hId;
	c=document.getElementById(HTMLId);
	p=c.parentNode;
	p.removeChild(c);
	cj.ajax.addonCallback('dashboardopennode','ptr='+nodePtr,dashOpenNodeCallback,'test');
}
function closeNode( nodePtr, HTMLId ) {
	var c,p,hId;
	c=document.getElementById(HTMLId);
	var p=c.parentNode;
	p.removeChild(c);
	hId='dashhelper'+nodeCnt++;
	navMakeHelper(hId);
	cj.ajax.addon('dashboardclosenode','ptr='+nodePtr,'',hId);
	dashResize();
}
function navMakeHelper( iconId ) {
	var w=document.getElementById('dashBoardWrapper');
	var e=document.createElement('div');
	e.setAttribute('id',iconId);
	e.setAttribute('style','position:absolute;top:0;left:0;');
	w.appendChild(e);
}
function navDropCallback(response,iconId){
	var e=document.getElementById(iconId);
	e.innerHTML=response;
	dashResize();
}
function navDrop(id,x,y){
	var iconId='dashhelper'+nodeCnt++;
	var w=document.getElementById('dashBoardWrapper');
	var e=document.createElement('div');
	var posX=(x-getPageOffsetLeft(w));
	var posY=(y-37-getPageOffsetTop(w));
	e.setAttribute('id',iconId);
	e.setAttribute('style','position:absolute;top:0;left:0;');
	w.appendChild(e);
	cj.ajax.addonCallback('dashboardnavdrop','id='+id+'&x='+posX+'&y='+posY,navDropCallback,iconId);
	// cj.ajax.addon('dashboardnavdrop','id='+id+'&x='+posX+'&y='+posY,'',iconId);
	// dashResize();
}
