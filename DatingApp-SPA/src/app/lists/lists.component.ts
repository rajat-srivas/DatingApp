import { Component, OnInit } from '@angular/core';
import { Pagination, PaginatedResult } from './../_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from './../_services/auth.service';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { User } from '../_models/user';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
users: User[];
pagination: Pagination;
likesParam: string;

  constructor(private userService: UserService, private authService: AuthService,
              private route: ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['user'].result;
      this.pagination = data['user'].pagination;
        });
    this.likesParam = 'Likers';
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likesParam).subscribe(
      (users: PaginatedResult<User[]>) => {
        this.users = users.result;
        this.pagination = users.pagination;
      },
      error => {
        this.alertify.error(error);
      }
    )
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUser();
  }


}
