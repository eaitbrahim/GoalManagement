import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-final-validation',
  templateUrl: './final-validation.component.html',
  styleUrls: ['./final-validation.component.css']
})
export class FinalValidationComponent implements OnInit {
  @Input() sheetStatus: string;
  @Input() ownerFullName: string;
  @Input() ownerComment: string;
  @Input() validatorFullName: string;
  @Input() validatorComment: string;
  @Input() ownerValidationDateTime: string;
  @Input() validatorValidationDateTime: string;
  @Output() addFinalEvaluationEvent = new EventEmitter<string>();
  comment: string = '';

  constructor() { }

  ngOnInit() {
  }

  addFinalEvaluation() {
    this.addFinalEvaluationEvent.emit(this.comment)
    this.comment = '';
  }
}
