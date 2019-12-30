import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { Axis } from './../../../_models/axis';
import { AxisPole } from './../../../_models/axisPole';

@Component({
  selector: 'app-axis-poles-weights-list',
  templateUrl: './axis-poles-weights-list.component.html',
  styleUrls: ['./axis-poles-weights-list.component.css']
})
export class AxisPolesWeightsListComponent implements OnInit {
  @Input() axisList: Axis[];
  @Input() isReadOnly: boolean;
  @Input() messages: any[];
  @Input() updatedAxisId: number;
  @Output() updateAxisPoleEvent = new EventEmitter<AxisPole>();

  constructor() { }

  ngOnInit() {
  }

  handleUpdateAxisPole(axisPole: AxisPole) {
    this.updateAxisPoleEvent.emit(axisPole);
  }
}
