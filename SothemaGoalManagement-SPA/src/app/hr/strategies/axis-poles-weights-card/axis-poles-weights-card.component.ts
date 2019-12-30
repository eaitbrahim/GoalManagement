import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { faCaretDown, faCaretUp } from '@fortawesome/free-solid-svg-icons';

import { AxisPole } from './../../../_models/axisPole';
import { Axis } from '../../../_models/axis';


@Component({
  selector: 'app-axis-poles-weights-card',
  templateUrl: './axis-poles-weights-card.component.html',
  styleUrls: ['./axis-poles-weights-card.component.css']
})
export class AxisPolesWeightsCardComponent implements OnInit {
  @Input() axis: Axis;
  @Input() isReadOnly: boolean;
  @Input() updatedAxisId: number;
  @Output() updateAxisPoleEvent = new EventEmitter<AxisPole>();
  axisPoleList: AxisPole[];
  isCollapsed: boolean;
  faCaretDown = faCaretDown;
  faCaretUp = faCaretUp;

  constructor() { }

  ngOnInit() {
    this.axisPoleList = this.axis.axisPoles;

    if (this.axis.id === this.updatedAxisId) this.isCollapsed = true;
    else this.isCollapsed = false;
  }

  handleUpdateAxisPole(axisPole: AxisPole) {
    this.updateAxisPoleEvent.emit(axisPole);
  }

  toggleAxis() {
    this.isCollapsed = !this.isCollapsed;
    this.updatedAxisId = this.axis.id;
  }
}
