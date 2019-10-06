import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from '../../_models/photo';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from './../../_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;
  loading = false;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertifyService: AlertifyService
  ) { }

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url:
        this.baseUrl +
        'users/' +
        this.authService.decodedToken.nameid +
        '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = file => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain,
          isApproved: res.isApproved
        };
        this.photos.push(photo);
        if (photo.isMain) {
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem(
            'user',
            JSON.stringify(this.authService.currentUser)
          );
        }
      }
    };

    this.uploader.onErrorItem = (item, response, status, headers) => {
      this.alertifyService.error(response);
    }
  }

  setMainPhoto(photo: Photo) {
    this.loading = true;
    this.userService
      .setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          this.loading = false;
          this.currentMain = this.photos.filter(p => p.isMain === true)[0];
          this.currentMain.isMain = false;
          photo.isMain = true;
          //this.getMemberPhotoChange.emit(photo.url);
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem(
            'user',
            JSON.stringify(this.authService.currentUser)
          );
        },
        error => {
          this.loading = false;
          this.alertifyService.error(error);
        }
      );
  }

  deletePhoto(id: number) {
    this.alertifyService.confirm(
      'Etes-vous sûr de vouloir supprimer cette photo?',
      () => {
        this.loading = true;
        this.userService
          .deletePhoto(this.authService.decodedToken.nameid, id)
          .subscribe(
            () => {
              this.loading = false;
              this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
              this.alertifyService.success('La photo a été supprimée');
            },
            error => {
              this.loading = false;
              this.alertifyService.error('Échec de la suppression de la photo');
            }
          );
      }
    );
  }
}
