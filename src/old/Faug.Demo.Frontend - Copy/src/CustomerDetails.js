import React, {Component} from 'react';

//This Component is a child Component of Customers Component
export default class CustomerDetails extends Component {

  constructor(props) {
    super(props);
    this.state = {}
  }

  //Function which is called when the component loads for the first time
  componentDidMount() {
  }

  //Function which is called whenver the component is updated
  componentDidUpdate(prevProps) {

  }

  render() {
    if (!this.state.customerDetails)
      return (<p>Loading Data</p>)
    return (<div></div>)
  }
}
