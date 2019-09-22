import React, { Component } from 'react';
import Auth from "./Auth";

const errorStyle = {
  color: 'red',
  weight: 700,
};

const formStyle = {
  input : {
    width: '60%',
    marginLeft: '40px'
  },
  select :{
    width: '60%',
  }
};

async function userLogin(username, password) {
    var auth = new Auth();
    var response = await auth.login(username, password)
    return response;
}

export class Login extends Component {
  static displayName = Login.name;
  constructor(props) {
    debugger;
    super(props);
    this.state =  {
        username : '',
        password : '',
        error : null,
    }
    this.handleChangePassword = this.handleChangePassword.bind(this);
    this.handleChangeUsername = this.handleChangeUsername.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  render () {
    return (
        <div>
        <h1>Login</h1>
            <div>
              <form style={formStyle} onSubmit={this.handleSubmit} key="login">
                <label>
                  Username
                  <input type="text" value={this.state.username} onChange={this.handleChangeUsername} />
                </label>
                <br />
                <label>
                  Password
                  <input type="password" value={this.state.password} onChange={this.handleChangePassword} />
                </label>
                <br />
                <input type="submit" data-date="test" value="Submit" />
              </form>
              {this.state.error &&
                <p style={errorStyle}>{this.state.error}</p>
              }
            </div>
        </div>
    );
  }

  handleChangePassword(event) {
    this.setState({error: null});
    this.setState({password: event.target.value});
  }

  handleChangeUsername(event) {
    this.setState({error: null});
    this.setState({username: event.target.value});
  }

  handleSubmit(event) {
    var self = this;
    event.preventDefault();
    userLogin(this.state.username, this.state.password)
      .then(function(response){
        if(response.error){
          self.setState({error: response.error});
        }
        else{
          self.props.history.push('/')
        }
      })
  }
}