import { Component, Input } from '@angular/core';
import { BirdImage, IBirdImage } from '../images-list.component';

@Component({
  selector: 'bird-image',
  templateUrl: './image.component.html',
  styleUrls: ['./image.component.scss']
})

export class ImageComponent  {
  @Input() birdImage?: BirdImage


}
