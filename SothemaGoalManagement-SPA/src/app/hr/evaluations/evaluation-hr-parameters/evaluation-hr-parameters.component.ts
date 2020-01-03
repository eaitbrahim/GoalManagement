import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { Parameters } from '../../../_models/parameters';

@Component({
  selector: 'app-evaluation-hr-parameters',
  templateUrl: './evaluation-hr-parameters.component.html',
  styleUrls: ['./evaluation-hr-parameters.component.css']
})
export class EvaluationHrParametersComponent implements OnInit {
  @Input() parameters: Parameters[] = [];
  @Output() addDateRangeEvent = new EventEmitter<any>();
  @Output() deleteDateRangeEvent = new EventEmitter<Parameters>();
  bsConfig: Partial<BsDatepickerConfig>;
  faTrash = faTrash;

  constructor() { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-dark-blue',
      dateInputFormat: 'YYYY-MM-DD'
    };
  }

  addParameters() {
    const event = (<HTMLInputElement>document.querySelector("#event")).value;
    const startEvent = (<HTMLInputElement>document.querySelector("#datesRange")).value.split('-')[0].trim();
    const endEvent = (<HTMLInputElement>document.querySelector("#datesRange")).value.split('-')[1].trim();
    this.addDateRangeEvent.emit({ event, startEvent, endEvent });
  }

  deleteParameters(parameters: Parameters) {
    this.deleteDateRangeEvent.emit(parameters);
  }
}
