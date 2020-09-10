import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { faTrash, faEdit } from '@fortawesome/free-solid-svg-icons';

import { Axis } from './../../../_models/axis';
import { HrService } from '../../../_services/hr.service';
import { AlertifyService } from '../../../_services/alertify.service';
import { AuthService } from '../../../_services/auth.service';
import { AxisModalComponent } from '../axis-modal/axis-modal.component';

@Component({
  selector: 'app-strategy-axis',
  templateUrl: './strategy-axis.component.html',
  styleUrls: ['./strategy-axis.component.css']
})
export class StrategyAxisComponent implements OnInit {
  @Input() axisList: Axis[];
  @Input() isReadOnly: boolean;
  @Output() addAxisEvent = new EventEmitter<Axis>();
  newAxis: any = {};
  bsModalRef: BsModalRef;
  public loading = false;
  faTrash = faTrash;
  faEdit = faEdit;

  constructor(private hrService: HrService,
    private authService: AuthService,
    private alertify: AlertifyService,
    private modalService: BsModalService) { }

  ngOnInit() {
  }

  addAxis() {
    this.addAxisEvent.emit(this.newAxis);
    this.newAxis.title = '';
    this.newAxis.description = '';
  }

  deleteAxis(id: number) {
    this.alertify.confirm('Supprimer',
      'Êtes-vous sûr de vouloir supprimer cet axe?',
      () => {
        this.loading = true;
        this.hrService
          .deleteAxis(id, this.authService.decodedToken.nameid)
          .subscribe(
            () => {
              this.loading = false;
              this.axisList.splice(
                this.axisList.findIndex(a => a.id === id),
                1
              );
              this.alertify.success('L\'axe a été supprimé');
            },
            error => {
              this.loading = false;
              this.alertify.error('Impossible de supprimer l\'axe');
            }
          );
      }
    );
  }

  editAxisModal(axis: Axis) {
    const initialState = {
      axis
    };

    this.bsModalRef = this.modalService.show(AxisModalComponent, { initialState, ignoreBackdropClick: true });
    this.bsModalRef.content.updateSelectedAxis.subscribe((updatedAxis) => {
      this.loading = true;
      this.hrService.updateAxis(axis.id, updatedAxis).subscribe(() => {
        this.loading = false;
        this.alertify.success('L\'axe été mis à jour.');
      }, error => {
        this.loading = false;
        this.alertify.error(error);
      })

    });
  }
}
