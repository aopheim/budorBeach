import { Component } from "@angular/core";
import { from, Observable } from "rxjs";
import { BirdImageDto, Client } from "src/services";

@Component({
  selector: "app-images-list",
  templateUrl: "./images-list.component.html",
  styleUrls: ["./images-list.component.scss"],
})
export class ImagesListComponent {
  constructor() {
    // this.birdImages$ = from(.getLatestBirdImages(6));
  }

  public birdImages$: Observable<BirdImageDto[]>;
}

