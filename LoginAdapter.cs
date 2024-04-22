using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace password_manager
{
    internal class LoginAdapter : BaseAdapter<ListViewElement>
    {
        public event EventHandler<ItemEventArgs> ModifyClicked;
        public event EventHandler<ItemEventArgs> DeleteClicked;
        List<ListViewElement> listViewElements;
        string type;
        Context context;
        public LoginAdapter(Context context,List<ListViewElement> listViewElements, string type)
        {
            this.listViewElements = listViewElements;
            this.type = type;
            this.context = context;
        }
        public override ListViewElement this[int position]
        {
            get
            {
                return listViewElements[position];
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return new Java.Lang.String(listViewElements[position]._Id);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.password_listelement, parent, false);

                var name = view.FindViewById<TextView>(Resource.Id.name);
                var email = view.FindViewById<TextView>(Resource.Id.email);
                var image = view.FindViewById<ImageView>(Resource.Id.imageview);
                var more = view.FindViewById<ImageView>(Resource.Id.more);
                more.Tag = position;
                more.Click -= MoreImageView_Click; 
                more.Click += MoreImageView_Click;
                view.Tag = new LoginAdapterViewHolder() { Name = name, Description = email, Image = image };
            }

            var holder = (LoginAdapterViewHolder)view.Tag;

            holder.Name.Text = listViewElements[position].Name;
            holder.Description.Text = listViewElements[position].Username;
            switch (type)
            {
                case "3":
                    holder.Image.SetImageResource(Resource.Drawable.notes);
                    break;
                case "2":
                    holder.Image.SetImageResource(Resource.Drawable.identify);
                    break;
                case "1":
                    holder.Image.SetImageResource(Resource.Drawable.card);
                    break;
                case "0":
                    holder.Image.SetImageResource(Resource.Drawable.user);
                    break;
                default:
                    break;
            }

            return view;
        }
        protected virtual void OnModifyClicked(ItemEventArgs e)
        {
            ModifyClicked?.Invoke(this, e);
        }
        protected virtual void OnDeleteClicked(ItemEventArgs e)
        {
            DeleteClicked?.Invoke(this, e);
        }
        void MoreImageView_Click(object sender, EventArgs e)
        {
            ImageView more = sender as ImageView;
            if (more == null)
                return;

            int position = (int)more.Tag;
            PopupMenu menu = new PopupMenu(context, more);
            menu.MenuInflater.Inflate(Resource.Menu.more_menu, menu.Menu);
            menu.MenuItemClick += (s, args) =>
            {
                // Itt kezeld a menü elemek kattintását
                if (args.Item.ItemId == Resource.Id.deleteOption)
                {
                    OnDeleteClicked(new ItemEventArgs { Position = position });
                }
                else if (args.Item.ItemId == Resource.Id.editOption)
                {
                    OnModifyClicked(new ItemEventArgs { Position = position });
                }
            };
            menu.Show();
        }
        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return listViewElements.Count;
            }
        }

    }

    internal class LoginAdapterViewHolder : Java.Lang.Object
    {
        public TextView Name { get; set; }
        public TextView Description { get; set; }
        public ImageView Image { get; set; }

    }
    public class ItemEventArgs : EventArgs
    {
        public int Position { get; set; }
    }
}