import { Component, Input } from "@angular/core";
import { BirdImageDto } from "src/services";

@Component({
  selector: "bird-image",
  templateUrl: "./image.component.html",
  styleUrls: ["./image.component.scss"],
})
export class ImageComponent {
  @Input() birdImage?: BirdImageDto;
}
