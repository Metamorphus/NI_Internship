import { Injectable } from '@angular/core';
import { Item } from './item';
import { ITEMS } from './mock-items';

@Injectable()
export class ItemService 
{
	items = ITEMS;

	getItems() 
	{
		return ITEMS;
	}

	decrement(item: Item) { if (item.quantity > 1) item.quantity--; }
	increment(item: Item) { item.quantity++; }

	removeItem(index: number)
	{
		console.log(index);
		this.items.splice(index, 1);
	}

	getRetailPrice()
	{
		var total = 0;
		for (var i = 0; i < this.items.length; i++)
		{
			var thisItem = this.items[i];
			total += thisItem.itemPrice * thisItem.quantity;
		}
		return total;
	}
}