package com.g2d.studio;

import java.awt.BorderLayout;
import java.awt.Image;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.image.BufferedImage;
import java.util.HashSet;
import java.util.Vector;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JList;
import javax.swing.JScrollPane;
import javax.swing.JToolBar;

import com.g2d.studio.Studio.ProgressForm;
import com.g2d.studio.fileobj.FileObject;
import com.g2d.studio.fileobj.FileObjectView;
import com.g2d.studio.gameedit.entity.IProgress;
import com.g2d.studio.io.File;
import com.g2d.studio.res.Res;
import com.g2d.studio.sound.SoundFile;
import com.g2d.studio.sound.SoundList;
import com.g2d.studio.swing.G2DList;
import com.g2d.studio.swing.G2DListItem;
import com.g2d.studio.swing.G2DTreeListView;
import com.g2d.studio.swing.G2DWindowToolBar;

import com.g2d.awt.util.*;



@SuppressWarnings("serial")
public abstract class ManagerFormTreeList<T extends FileObject> extends ManagerForm implements ActionListener
{	
	final protected FileObjectView<T>	list;
	final protected File 				save_list_file;
	final protected File 				res_root;
	
	final protected G2DWindowToolBar	tool_bar 		= new G2DWindowToolBar(this);
	
	final protected JButton				btn_refresh 	= new JButton(Tools.createIcon(Res.icon_refresh));
	final protected JButton				btn_view		= new JButton(Tools.createIcon(Res.icon_event));

	int view_index = 0;
	int[] view_modes = new int[]{
		JList.HORIZONTAL_WRAP,
		JList.VERTICAL,
		JList.VERTICAL_WRAP,
	};
	
	public ManagerFormTreeList(
			Studio studio, 
			ProgressForm progress, 
			String title, 
			BufferedImage icon, 
			File res_root, 
			File save_list_file) 
	{
		super(studio, progress, title, icon);
		
		this.save_list_file = save_list_file;
		this.res_root		= res_root;
		
		this.btn_refresh.setToolTipText("刷新");
		this.btn_refresh.addActionListener(this);
		
		this.btn_view.setToolTipText("查看");
		this.btn_view.addActionListener(this);
		
		this.tool_bar.add(btn_refresh);
		this.tool_bar.add(btn_view);
		
		this.add(tool_bar, BorderLayout.NORTH);		
		
		this.list = createList(res_root, save_list_file, progress);
		this.add(new JScrollPane(list), BorderLayout.CENTER);
		
		saveAll(null);
	}

	@Override
	public void actionPerformed(ActionEvent e) {
		if (e.getSource() == tool_bar.save) {
			saveAll(null);
		}
		else if (e.getSource() == btn_refresh) {
			list.refresh(new SaveProgressForm());
			saveAll(null);
		}
		else if (e.getSource() == btn_view) {
			view_index ++;
			view_index %= view_modes.length;
			this.list.getList().setLayoutOrientation(view_modes[view_index]);
		}
	}
	
	abstract protected FileObjectView<T> createList(
			File res_root, 
			File save_list_file, 
			ProgressForm progress);
	
	public Vector<T> getNodes() {
		return list.getNodes();
	}
	
	public T getNode(String node_name) {
		return list.getNode(node_name);
	}
	
	
	public void saveAll(IProgress progress) 
	{
		String sb = list.getListFileData();
		save_list_file.writeUTF(sb.toString());
	}
	
	@Override
	public void saveSingle() throws Throwable {}
}