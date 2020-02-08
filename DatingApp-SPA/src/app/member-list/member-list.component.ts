import { Component, OnInit } from '@angular/core';
import { User } from '../_models/user';
import { UserService } from './../_services/user.service';
import { AlertifyService } from './../_services/alertify.service';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  pageNumber = 1;
  pageSize = 15;
  pagination: Pagination ;
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value:'male', display: 'Males'}, {value: 'female', display: 'Females'}];
  userParams: any = {};

  constructor(private userService: UserService, private alertify: AlertifyService , private route: ActivatedRoute) { }

  ngOnInit() {
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUser(this.pageNumber, this.pageSize);
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUser(this.pagination.currentPage, this.pagination.itemsPerPage);
  }

  loadUser(pNumber: number, pSize: number) {
    console.log(this.userParams);
    this.userService.getUsers(pNumber, pSize, this.userParams).subscribe(
      (users: PaginatedResult<User[]>) => {
        this.users = users.result;
        this.pagination = users.pagination;
        console.log('rajat');
        console.log(users.pagination);
      },
      error => {
        this.alertify.error(error);
      }
    )
  }

  resetFilters(){
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUser(this.pageNumber, this.pageSize);
  }

}
