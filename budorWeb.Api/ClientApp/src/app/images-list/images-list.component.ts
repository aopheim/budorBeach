import { Component, ModuleWithComponentFactories, OnInit } from '@angular/core';

@Component({
  selector: 'app-images-list',
  templateUrl: './images-list.component.html',
  styleUrls: ['./images-list.component.scss']
})
export class ImagesListComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  birdImage = new BirdImage("https://upload.wikimedia.org/wikipedia/commons/thumb/4/45/Eopsaltria_australis_-_Mogo_Campground.jpg/1200px-Eopsaltria_australis_-_Mogo_Campground.jpg",
   "06.11.2021, 08:30:43")

}

export class BirdImage implements IBirdImage {
  constructor(public url: string,  public date: string){
  }

}

export interface IBirdImage {
  url: string,
  date: string
}
