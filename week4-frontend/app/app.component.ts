import { Component, OnInit } from '@angular/core';

import { PromoComponent } from './promo.component'

import { Item } from './item';
import { ItemService } from './item.service';

@Component({
	selector: 'my-app',
	template: `
	<div class="container">
		<div class="col-sm-12">
			<h2 class="text-center">Shopping cart</h2>
			<table align="center">
				<thead>
					<th style="width:61%">Item name</th>
					<th style="width:13%">Price $</th>
					<th style="width:13%">Quantity</th>
					<th style="width:13%">Total $</th>
				</thead>

				<tbody>
				<tr *ngFor="let item of items; let i=index">
					<td class="vertical-align-center">
						<img class="image-preview-card" src={{item.url}}> {{item.name}} 
						<button (click)="itemService.removeItem(i)" class="delete-button"><span class="glyphicon glyphicon-trash"></span></button>
					</td>
					<td class="vertical-align-center">{{item.itemPrice}}</td>
					<td class="vertical-align-center">
						<button (click)="itemService.decrement(item)" type="button">-</button>
						<input size="10px" [(ngModel)]="item.quantity" value="{{item.quantity}}" type="number" maxlength="1"/>
						<button (click)="itemService.increment(item)" type="button">+</button>
					</td>
					<td class="vertical-align-center">{{item.quantity*item.itemPrice}}</td>
				</tr>
				</tbody>
			</table>
		</div>

		<div class="col-sm-12">
	    <div align="right">
	      <h3>Retail Price: {{ itemService.getRetailPrice() }}</h3>
	      <promo-area></promo-area>
	    </div>
	    </div>
	</div>
	`,
	directives: [PromoComponent],
	providers: [ItemService]
})

export class AppComponent implements OnInit
{
	items: Item[];

	constructor(private itemService: ItemService) { }

	getItems() 
	{
	  this.items = this.itemService.getItems();
	}

	ngOnInit() {
	  this.getItems();
	}
}

