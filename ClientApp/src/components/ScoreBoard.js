import React, { Component } from 'react';
import ApiHelper from './ApiHelper';

export class ScoreBoard extends Component {
  static displayName = ScoreBoard.name;

  constructor (props) {
    super(props);
    this.state = { forecasts: [], loading: true };
    /*
    this.apiHelper = new ApiHelper();
    var query = `{"query" : "{ leaderBoard{teamScores { teamId, teamName, teamStepCount } }}"}`
    this.apiHelper.GraphQlApiHelper(query)
    .then(data => {
        this.setState({ forecasts: data.leaderBoard.teamScores, loading: false });
    })
    */
  }

  static renderForecastsTable (forecasts) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th></th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map(forecast =>
            <tr key={forecast.teamId}>
              <td>{forecast.teamName}</td>
              <td>{forecast.teamStepCount}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render () {
    let contents = <p><em>Loading...</em></p>
    /*this.state.loading
      ? <p><em>Loading...</em></p>
      : ScoreBoard.renderForecastsTable(this.state.forecasts);
      */

    return (
      <div>
        <h2>Latest Scores</h2>
        {contents}
      </div>
    );
  }
}
