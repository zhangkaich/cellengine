
#ifndef _MF_CELL_MAP
#define _MF_CELL_MAP

#include "MFType.h"
#include "MFGraphics2D.h"
#include "MFGameObject.h"
#include "MFNode.h"
#include <vector>

#include "cocos2d.h"

namespace mf
{
	using namespace std;
	
	typedef struct CC_DLL CMapLayer
	{
		vector<vector<int> > TileMatrix2D;
		vector<vector<int> > FlipMatrix2D;
		vector<vector<int> > FlagMatrix2D;

		int getTileValue(int x, int y)
		{
			return TileMatrix2D[y][x];
		}
		int getFlipValue(int x, int y)
		{
			return FlipMatrix2D[y][x];
		}
		int getFlagValue(int x, int y)
		{
			return FlagMatrix2D[y][x];
		}
	} 
	CMapLayer ;

	/**
	* @author yifeizhang
	* @since 2006-11-30 
	* @version 1.0
	*/
	class CC_DLL CMapMeta
	{
	protected:

		int		mCellW;
		int		mCellH;
		int		mWidthBlock;
		int		mHeightBlock;
		int		mWidthPixel;
		int		mHeightPixel;

		bool	mIsCyc;

		ITiles* mTiles;

		vector<CMapLayer> mLayers;
		
		vector<CCD> mBlockTypes;

	public:

		CMapMeta(ITiles* tiles, int cellw, int cellh, int layerCount, int widthBlock, int heightBlock);

		int fillMapCell(int layer, int bx, int by, int tileID, int flip, int flag);
		int defineBlockType(int index,
			int BlocksType, int BlocksMask, 
			int BlocksX1, int BlocksY1, 
			int BlocksX2, int BlocksY2);

		ITiles* getTiles();

		int getWidthPixel();
		int getHeighPixel() ;
		int getWidthBlock() ;
		int getHeighBlock() ;
		int getCellW() ;
		int getCellH() ;
		int getLayerCount() ;

		int getTile(int layer, int bx,int by);
		int getFlip(int layer, int bx,int by);
		int getFlag(int layer, int bx,int by);
		
		void renderCell(Graphics2D* g, int layer, int sbx, int sby, int dx, int dy);

		void renderBath(Graphics2D* g, u32 cax, u32 cay, u32 caw, u32 cah);

	};

	class  CC_DLL CCellMap : public MFNode
	{
	public:
		virtual void locateCamera(float x, float y) = 0;

		virtual void moveCamera(float x, float y) = 0;
	};

	class  CC_DLL CCellMapDirect : public CCellMap
	{
	protected:

		CMapMeta* mMeta;
		
		Rectangle2D mCamera;

	public:
		
		CCellMapDirect(CMapMeta* mMeta, float width, float height);

		virtual ~CCellMapDirect();

		void setCameraSize(float width, float height);

		virtual void locateCamera(float x, float y);
		
		virtual void moveCamera(float x, float y);

		virtual void draw(void);
		
	protected:

		// call in draw , implements replace
		virtual void render(Graphics2D *g);
	};

	class  CC_DLL CCellMapBuffer : public CCellMap
	{
	protected:
		CMapMeta*			mMeta;
		IImage				*mBuffer;
		BufferedGraphis2D	*mBG;
		Rectangle2D			mCamera;
		
	public:

		CCellMapBuffer(CMapMeta* mMeta, float width, float height);

		virtual ~CCellMapBuffer();

		virtual void locateCamera(float x, float y);

		virtual void moveCamera(float x, float y);

		virtual void draw(void);

		virtual void update(ccTime dt);

	protected:

		// call in draw , implements replace
		virtual void render(Graphics2D *g);
	};
};

#endif // #define _MF_CELL_MAP