import { Component } from '@angular/core';

import { Item } from './item';
import { ItemService } from './item.service';

@Component({
  selector: 'promo-area',
  template: `
    <label>Promocode: </label> <input [(ngModel)]="promocode" value="ni16%" />
    <h3>Total Price: {{ getTotalPrice()}}</h3>
  `
})


export class PromoComponent {

  constructor(private itemService: ItemService) { }

  promocode: string;
  promocodeFunction: { [key:string]:Function; } = {
    "ni16%": ((price: number) => price*5/6),
    "ni33%": ((price: number) => price*2/3),
    "ni10$": ((price: number) => price-10)
  };

  getTotalPrice()
  {
    console.log("entered function");
    var func = this.promocodeFunction[this.promocode];
    if (func == undefined)
      return this.itemService.getRetailPrice();
    else
      return Math.floor(func(this.itemService.getRetailPrice()));
  }
}