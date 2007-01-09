package com.morefuntek.cell.Game;

import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;

import com.morefuntek.cell.CObject;

public class CWorldMini extends CObject {

	public int[] MapColor;
	public int[] SprColor;
	public int BorderColor = 0xff000000;
	public int CameraColor = 0xff00ff00;
	
	public boolean ShowSpr = true;
	public boolean ShowMap = true;
	public boolean ShowCam = true;
	
	public int X = 0;
	public int Y = 0;
	
	CWorld World;

	
	int W;
	int H;
	int CW;
	int CH;

	int WW;
	int WH;
	
	Image Buffer;
	Graphics bg;
	
	public CWorldMini(
			CWorld world,
			int width,int height,
			int cellW,int cellH,
			int colorKeyMapPos,
			int colorKeySprPos){
		
		int[] c = new int[1];
		
		int[] sprColor = new int[world.getSpriteCount()];
		int[] mapColor = new int[world.getMap().getAnimates().getCount()];
		
       	for(int i=0;i<sprColor.length;i++){
       		try {
				Image k = world.getSprite(i).getAnimates().getFrameImage(0, 0);
				k.getRGB(c, 0, 1, 
						colorKeySprPos%k.getWidth(), 
						colorKeySprPos/k.getWidth(), 
						1, 1);
				sprColor[i] = c[0];
			} catch (RuntimeException e){
				sprColor[i] = 0xffffffff;
			}
       	}
       	
    	for(int i=0;i<mapColor.length;i++){
    		try {
				Image k = world.getMap().getAnimates().getFrameImage(i, 0);
				k.getRGB(c, 0, 1, 
						colorKeyMapPos%k.getWidth(), 
						colorKeyMapPos/k.getWidth(), 
						1, 1);
				mapColor[i] = c[0];
			} catch (RuntimeException e){
				mapColor[i] = 0xff00ff00;
			}
       	}
    	
		World = world;
		W = width;
		H = height;
		CW = cellW;
		CH = cellH;
		WW = W*World.getMap().getCellW()/CW;
		WH = H*World.getMap().getCellH()/CH;
		
		MapColor = mapColor;
		SprColor = sprColor;
		
		Buffer = Image.createImage(
					world.Map.getWCount()*CW, 
					world.Map.getHCount()*CH);
		bg = Buffer.getGraphics();
		
		for (int by = 0; by < world.getMap().getHCount(); by++) {
			for (int bx = 0; bx < world.getMap().getWCount(); bx++) {
				bg.setColor(MapColor[World.Map.getTile(bx, by)]);
				bg.fillRect(bx*CW, by*CH, CW, CH);
			}
		}
		
	}
	
	public CWorldMini(
			CWorld world,
			int width,int height,
			int cellW,int cellH,
			int[] mapColor,
			int[] sprColor){

		World = world;
		W = width;
		H = height;
		CW = cellW;
		CH = cellH;
		WW = W*World.getMap().getCellW()/CW;
		WH = H*World.getMap().getCellH()/CH;
		
		MapColor = mapColor;
		SprColor = sprColor;
		
		Buffer = Image.createImage(
					world.Map.getWCount()*CW, 
					world.Map.getHCount()*CH);
		bg = Buffer.getGraphics();
		
		for (int by = 0; by < world.getMap().getHCount(); by++) {
			for (int bx = 0; bx < world.getMap().getWCount(); bx++) {
				bg.setColor(MapColor[World.Map.getTile(bx, by)]);
				bg.fillRect(bx*CW, by*CH, CW, CH);
			}
		}
		
	}

	
	
	public int getWidth(){
		return W;
	}
	public int getHeight(){
		return H;
	}
	public int getWorldWidth(){
		return WW;
	}
	public int getWorldHeight(){
		return WH;
	}
	
	public void render(Graphics g,int x,int y){
		try{
			int cx = g.getClipX();
			int cy = g.getClipY();
			int cw = g.getClipWidth();
			int ch = g.getClipHeight();
	    	g.setClip(x,y,W,H);
	    	
	    	if(X<0)X=0;
	    	if(X+WW>World.getMap().getWidth())X=World.getMap().getWidth()-WW;
	    	if(Y<0)Y=0;
	    	if(Y+WH>World.getMap().getHeight())Y=World.getMap().getHeight()-WH;
	    	
			if(ShowMap){
				AScreen.drawRegion(g, Buffer, 
						(X)*CW/World.getMap().CellW, 
						(Y)*CH/World.getMap().CellH,
						W, H, 
						x, y);
			}
			if(ShowSpr){
				for(int i=0;i<World.getSpriteCount();i++){
					if(World.getSprite(i).OnScreen && World.getSprite(i).Visible){
						g.setColor(SprColor[i]);
						g.fillRect(
								x -CW/2 + (World.getSprite(i).X-X)*CW/World.getMap().CellW, 
								y -CH/2 + (World.getSprite(i).Y-Y)*CH/World.getMap().CellH, 
								CW, 
								CH);
					}
				}
			}
			if(ShowCam){
				g.setColor(CameraColor);
				g.drawRect(
						x -CW/2 + (World.getCamera().getX()-X)*CW/World.getMap().CellW, 
						y -CH/2 + (World.getCamera().getY()-Y)*CH/World.getMap().CellH, 
						World.getCamera().getWidth() *CW/World.getMap().CellW, 
						World.getCamera().getHeight()*CH/World.getMap().CellH);
			}
			
			g.setColor(BorderColor);
			g.drawRect(x, y, W-1, H-1);
			
			g.setClip(cx,cy,cw,ch);
			
		}catch(RuntimeException e){
		}
		
	}
	
	
}