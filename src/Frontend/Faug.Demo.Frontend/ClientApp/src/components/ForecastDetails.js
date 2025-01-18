import React, { Component } from 'react';
import { useParams } from 'react-router';

export class ForecastDetails extends Component {

  constructor(props) {
    super(props);
      this.state = { forecasts: [], city: this.props.match.params.id, loading: true };
  }

  componentDidMount()
  {
    this.populateWeatherData(this.state.city);
  }

  static renderForecastsTable(forecasts) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Datetime</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map(forecast =>
            <tr key={forecast.date}>
                  <td>{forecast.dateTime}</td>
              <td>{forecast.temperatureC}</td>
              <td>{forecast.temperatureF}</td>
              <td>{forecast.summary}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
        : ForecastDetails.renderForecastsTable(this.state.forecasts);

    return (
        <div>
            <h1 id="tabelLabel">Weather forecast {this.state.city}</h1>
        {contents}
      </div>
    );
  }

    async populateWeatherData(city) {
        const response = await fetch('/api/weather/locations/' + city );
        const data = await response.json();
        this.setState({ forecasts: data, loading: false });
  }
}
