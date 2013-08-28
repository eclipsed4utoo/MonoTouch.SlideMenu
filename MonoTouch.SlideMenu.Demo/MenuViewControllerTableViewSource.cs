using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SlideMenu;

namespace MonoTouch.SlideMenu.Demo
{
	public class MenuViewControllerTableViewSource : UITableViewSource 
	{
		const int NUMBER_OF_SECTIONS = 3;
		const int NUMBER_OF_ROWS = 10;
		
		UIViewController controller;
		
		public MenuViewControllerTableViewSource (UIViewController controller)
		{
			this.controller = controller;
		}			
		
		public override int NumberOfSections (UITableView tableView) {
			return NUMBER_OF_SECTIONS;
		}
		
		public override int RowsInSection (UITableView tableview, int section) {
			return NUMBER_OF_ROWS;
		}
		
		const string CellIdentifier = "Cell";
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);
			
			if (cell == null) {
				cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
			}
			
			cell.TextLabel.Text = string.Format("Section: {0} Row: {1}", indexPath.Section, indexPath.Row);
			
			return cell;
		}
		
		//			- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
		//			{
		//				if (self.slideMenuController)
		//				{
		//					DetailsViewController *detailsVC = [DetailsViewController new];
		//					detailsVC.detailedObject = indexPath;
		//					
		//					if (indexPath.section == 0 && indexPath.row == 1)
		//					{
		//						[detailsVC setOnShowMenuButtonClicked:^{
		//							[self dismissModalViewControllerAnimated:YES];
		//						 }];
		//						[self presentViewController:detailsVC animated:YES completion:nil];
		//					}
		//					else
		//					{
		//						UINavigationController *navController = [[UINavigationController alloc] initWithRootViewController:detailsVC];
		//						[self.slideMenuController setContentViewController:navController animated:YES completion:nil];
		//						[navController release];
		//					}
		//					
		//					[detailsVC release];
		//				}
		//				else
		//				{
		//					[tableView deselectRowAtIndexPath:indexPath animated:YES];
		//				}
		//			}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var smc = controller.SlideMenuController ();
			if (smc != null) {
				smc.ToggleRightMenuAnimated ();
			}

			tableView.DeselectRow(indexPath, true);
		}
	}
}
