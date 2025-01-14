import React, {Component} from 'react';
import CustomerDetails from './CustomerDetails'

export default class Customers extends Component {

  constructor(props) {
    super(props)
    this.state = {
      selectedCustomer: 1
    }
  }

  //function which is called the first time the component loads
  componentDidMount() {
  }

  render() {
    if (!this.state.customerList)
      return (<p>Loading data</p>)
    return (<div></div>)
  }

}
