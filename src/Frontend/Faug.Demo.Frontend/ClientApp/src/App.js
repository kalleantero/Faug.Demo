import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { LocationListing } from './components/LocationListing';
import { ForecastDetails } from './components/ForecastDetails';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route exact path="/locations" component={LocationListing}>
        </Route>
            <Route exact path="/locations/:id" component={ForecastDetails}>
        </Route>
      </Layout>
    );
  }
}
